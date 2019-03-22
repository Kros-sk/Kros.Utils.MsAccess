using Kros.MsAccess.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Kros.Data.BulkActions.MsAccess
{
    /// <summary>
    /// Class for generating CSV files for Microsoft Access bulk insert.
    /// </summary>
    internal class CsvFileWriter : IDisposable
    {
        #region Static

        private const string StringTrue = "1";
        private const string StringFalse = "0";
        private const string DecimalNumberFormat = "0.0###########";
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private const char DefaultValueDelimiter = ',';
        private const char DefaultStringQuote = '"';
        private static readonly Encoding DefaultFileEncoding = Encoding.UTF8;

        protected static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of writer with specified output file <paramref name="filePath" /> and encoding
        /// <paramref name="encoding" />.
        /// </summary>
        /// <param name="filePath">Path to the output file.</param>
        /// <param name="encoding">Encoding of the output file.</param>
        /// <param name="append">Specifies if output file wil be ovrewritten (<see langword="false"/>), or appended to
        /// (<see langword="true"/>) if it already exists.</param>
        /// <exception cref="ArgumentException">The value of súboru <paramref name="filePath" /> is empty string, or string
        /// containing whitespace characters only.</exception>
        public CsvFileWriter(string filePath, Encoding encoding, bool append)
        {
            FilePath = Check.NotNullOrWhiteSpace(filePath, nameof(filePath));
            FileEncoding = encoding;
            _writer = new StreamWriter(FilePath, append, FileEncoding);
            StringQuote = DefaultStringQuote;
        }

        /// <summary>
        /// Creates an instance of writer with specified output file <paramref name="filePath" /> and UTF-8 encoding.
        /// </summary>
        /// <param name="filePath">Path to the output file.</param>
        /// <param name="append">Specifies if output file wil be ovrewritten (<see langword="false"/>), or appended to
        /// (<see langword="true"/>) if it already exists.</param>
        /// <exception cref="ArgumentException">The value of súboru <paramref name="filePath" /> is empty string, or string
        /// containing whitespace characters only.</exception>
        public CsvFileWriter(string filePath, bool append)
            : this(filePath, DefaultFileEncoding, append)
        {
        }

        /// <summary>
        /// Creates an instance of writer with specified output file <paramref name="filePath" /> and encoding
        /// <paramref name="codePage" />.
        /// </summary>
        /// <param name="filePath">Path to the output file.</param>
        /// <param name="codePage">Code page number. For example value for Windows central europe is 1250</param>
        /// <param name="append">Specifies if output file wil be ovrewritten (<see langword="false"/>), or appended to
        /// (<see langword="true"/>) if it already exists.</param>
        /// <exception cref="ArgumentException">The value of súboru <paramref name="filePath" /> is empty string, or string
        /// containing whitespace characters only.</exception>
        public CsvFileWriter(string filePath, int codePage, bool append)
            : this(filePath, Encoding.GetEncoding(codePage), append)
        {
        }

        /// <summary>
        /// Creates an instance of writer where output file is random file in system's temporary folder.
        /// </summary>
        /// <remarks>Path to the generated output file is accessible in <see cref="FilePath" /> property.</remarks>
        public CsvFileWriter()
            : this(Path.GetTempFileName(), DefaultFileEncoding, true)
        {
        }

        public CsvFileWriter(Encoding encoding)
            : this(Path.GetTempFileName(), encoding, true)
        {
        }

        public CsvFileWriter(Encoding encoding, bool append)
            : this(Path.GetTempFileName(), encoding, append)
        {
        }

        public CsvFileWriter(int codePage)
            : this(Path.GetTempFileName(), Encoding.GetEncoding(codePage), true)
        {
        }

        public CsvFileWriter(int codePage, bool append)
            : this(Path.GetTempFileName(), Encoding.GetEncoding(codePage), append)
        {
        }

        #endregion

        #region Common

        /// <summary>
        /// Value separator in output file.
        /// </summary>
        /// <value>Default value is comma (<b>,</b>).</value>
        public char ValueDelimiter { get; set; } = DefaultValueDelimiter;

        private char _stringQuote;
        private string _stringQuoteSubstitute;

        /// <summary>
        /// Character for quoting strings.
        /// </summary>
        /// <value>Default value is quotation mark (<b>"</b>).</value>
        public char StringQuote
        {
            get { return _stringQuote; }
            set
            {
                _stringQuote = value;
                _stringQuoteSubstitute = new string(_stringQuote, 2);
            }
        }

        /// <summary>
        /// Text encoding of output file.
        /// </summary>
        /// <value>Some <see cref="System.Text.Encoding">Encoding</see>.</value>
        public Encoding FileEncoding { get; } = DefaultFileEncoding;

        /// <summary>
        /// Path to the output file.
        /// </summary>
        /// <value>String.</value>
        public string FilePath { get; }

        #endregion

        #region Writing CSV

        private readonly TextWriter _writer;

        /// <summary>
        /// Writes one record <paramref name="data"/> to the output file.
        /// </summary>
        /// <param name="data">List of data values.</param>
        /// <remarks>
        /// Whole one record/line is written to the output file. Individual values must be of correct .NET data type, because
        /// data type determines how the value will be written. <b>So for example it is bad to write float values as strings.</b>
        /// For conversion to correct string for output are used methods <c>ProcesXxxValue</c>.
        /// </remarks>
        public void Write(IEnumerable<object> data)
        {
            CheckDisposed();

            bool addValueDelimiter = false;
            foreach (var value in data)
            {
                if (addValueDelimiter)
                {
                    _writer.Write(ValueDelimiter);
                    _writer.Write(" ");
                }
                else
                {
                    addValueDelimiter = true;
                }

                if ((value != null) && (!object.ReferenceEquals(value, DBNull.Value)))
                {
                    WriteValue(value);
                }
            }
            _writer.WriteLine();
        }

        public void Write(IDataReader data)
        {
            Write(data, null);
        }

        public void Write(IDataReader data, IEnumerable<string> columnNames)
        {
            CheckDisposed();

            List<int> ordinals = GetColumnOrdinals(data, columnNames);
            while (data.Read())
            {
                WriteRecord(data, ordinals);
            }
        }

        private void WriteRecord(IDataReader data, List<int> columnOrdinals)
        {
            bool addValueDelimiter = false;

            foreach (int columnOrdinal in columnOrdinals)
            {
                var value = data.GetValue(columnOrdinal);

                if (addValueDelimiter)
                {
                    _writer.Write(ValueDelimiter);
                    _writer.Write(" ");
                }
                else
                {
                    addValueDelimiter = true;
                }

                if ((value != null) && (!object.ReferenceEquals(value, DBNull.Value)))
                {
                    WriteValue(value);
                }
            }
            _writer.WriteLine();
        }

        private List<int> GetColumnOrdinals(IDataReader data, IEnumerable<string> columnNames)
        {
            if (columnNames == null)
            {
                return new List<int>(Enumerable.Range(0, data.FieldCount));
            }

            var dataOrdinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < data.FieldCount; i++)
            {
                dataOrdinals.Add(data.GetName(i), i);
            }

            var ordinals = new List<int>();
            foreach (string name in columnNames)
            {
                if (dataOrdinals.TryGetValue(name, out int value))
                {
                    ordinals.Add(value);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(Resources.BulkInsertSourceColumnDoesNotExist, name));
                }
            }

            return ordinals;
        }

        /// <summary>
        /// Writes one value to the output file.
        /// </summary>
        /// <param name="value">Written data.</param>
        protected void WriteValue(object value)
        {
            TypeCode valueTypeCode = default(TypeCode);

            if (value is bool)
            {
                _writer.Write(ProcessBooleanValue((bool)value));
            }
            else if (value is DateTime)
            {
                _writer.Write(ProcessDateTimeValue((DateTime)value));
            }
            else if ((value is string) || (value is char))
            {
                _writer.Write(StringQuote);
                _writer.Write(ProcessStringValue(Convert.ToString(value)));
                _writer.Write(StringQuote);
            }
            else if (value is Guid)
            {
                _writer.Write(ProcessGuidValue((Guid)value));
            }
            else
            {
                valueTypeCode = Type.GetTypeCode(value.GetType());
                if (value.GetType().IsEnum)
                {
                    if (valueTypeCode == TypeCode.Int64)
                    {
                        _writer.Write(Convert.ToInt64(value));
                    }
                    else
                    {
                        _writer.Write(Convert.ToInt32(value));
                    }
                }
                else
                {
                    switch (valueTypeCode)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Int16:
                        case TypeCode.Byte:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.UInt16:
                        case TypeCode.SByte:
                            _writer.Write(value);
                            break;
                        case TypeCode.Single:
                            _writer.Write(ProcessSingleValue(Convert.ToSingle(value)));
                            break;
                        case TypeCode.Double:
                            _writer.Write(ProcessDoubleValue(Convert.ToDouble(value)));
                            break;
                        case TypeCode.Decimal:
                            _writer.Write(ProcessDecimalValue(Convert.ToDecimal(value)));
                            break;

                        default:
                            throw new ArgumentException(
                                string.Format(Resources.UnknownDataTypeForCsv, value.GetType().FullName, value.ToString()));
                    }
                }
            }
        }

        protected virtual string ProcessStringValue(string value)
        {
            if (value.Contains(StringQuote))
            {
                return value.Replace(StringQuote.ToString(), _stringQuoteSubstitute);
            }
            return value;
        }

        protected virtual string ProcessGuidValue(Guid value)
        {
            return value.ToString();
        }

        protected virtual string ProcessDoubleValue(double value)
        {
            return value.ToString(DecimalNumberFormat, Culture.NumberFormat);
        }

        protected virtual string ProcessSingleValue(float value)
        {
            return value.ToString(DecimalNumberFormat, Culture.NumberFormat);
        }

        protected virtual string ProcessDecimalValue(decimal value)
        {
            return value.ToString(DecimalNumberFormat, Culture.NumberFormat);
        }

        protected virtual string ProcessDateTimeValue(DateTime value)
        {
            return StringQuote + value.ToString(DateTimeFormat) + StringQuote;
        }

        protected virtual string ProcessBooleanValue(bool value)
        {
            return value ? StringTrue : StringFalse;
        }

        #endregion

        #region IDisposable

        protected void CheckDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _writer.Dispose();
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
