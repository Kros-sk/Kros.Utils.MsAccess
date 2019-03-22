using Kros.Data.Schema;
using Kros.Data.Schema.MsAccess;
using Kros.MsAccess.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions.MsAccess
{
    /// <summary>
    /// The calss for fast bulk insert big amount of data into Microsoft Access database.
    /// </summary>
    /// <remarks>
    /// In the background, it creates a text CSV file with data which are inserted.
    /// </remarks>
    public class MsAccessBulkInsert : IBulkInsert
    {
        #region Nested Types

        private class MsAccessBulkInsertColumn
        {
            public MsAccessBulkInsertColumn(string sourceColumnName, BulkInsertColumnMapping mapping)
            {
                SourceColumnName = Check.NotNullOrWhiteSpace(sourceColumnName, nameof(sourceColumnName));
                Mapping = mapping;
            }

            public MsAccessBulkInsertColumn(
                string sourceColumnName,
                string destinationColumnName,
                BulkInsertColumnType columnType)
            {
                SourceColumnName = Check.NotNullOrWhiteSpace(sourceColumnName, nameof(sourceColumnName));
                DestinationColumnName = Check.NotNullOrWhiteSpace(destinationColumnName, nameof(destinationColumnName));
                ColumnType = columnType;
            }

            public string SourceColumnName { get; }
            public string DestinationColumnName { get; set; }
            public BulkInsertColumnMapping Mapping { get; set; }
            public BulkInsertColumnType ColumnType { get; set; }

            public override string ToString()
            {
                return $"{SourceColumnName} -> {DestinationColumnName} ({ColumnType})";
            }
        }

        #endregion

        #region Constants

        /// <summary>
        /// Default value separator for CSV file: comma (<b>,</b>).
        /// </summary>
        public const char DefaultValueDelimiter = ',';

        /// <summary>
        /// Default code page: <see cref="Utf8CodePage"/>.
        /// </summary>
        public const int DefaultCodePage = Utf8CodePage;

        /// <summary>
        /// UTF-8 code page: 65001.
        /// </summary>
        public const int Utf8CodePage = 65001;

        /// <summary>
        /// Windows Central Europe code page: <b>1250</b>.
        /// </summary>
        public const int WindowsCentralEuropeCodePage = 1250;

        /// <summary>
        /// ANSI code page.
        /// </summary>
        public const int AnsiCodePage = int.MaxValue;

        /// <summary>
        /// OEM code page.
        /// </summary>
        public const int OemCodePage = int.MaxValue - 1;

        #endregion

        #region Private fields

        private OleDbConnection _connection;
        private readonly bool _disposeOfConnection = false;
        private readonly List<MsAccessBulkInsertColumn> _columns = new List<MsAccessBulkInsertColumn>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance with database connection specifiend in <paramref name="connectionString"/>
        /// and default settings for CSV file.
        /// </summary>
        /// <param name="connectionString">Connection string for the database connection where the data will be inserted.</param>
        public MsAccessBulkInsert(string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            ExternalTransaction = null;
            CodePage = DefaultCodePage;
            ValueDelimiter = DefaultValueDelimiter;
            _connection = new OleDbConnection(connectionString);
            _disposeOfConnection = true;
        }

        /// <summary>
        /// Creates a new instance with database connection <paramref name="connection"/> and default settings for CSV file.
        /// </summary>
        /// <param name="connection">Database connection where the data will be inserted. The connection mus be opened.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public MsAccessBulkInsert(OleDbConnection connection)
            : this(connection, null, DefaultCodePage, DefaultValueDelimiter)
        {
        }

        /// <summary>
        /// Creates a new instance with database connection <paramref name="connection"/>, transaction
        /// <paramref name="externalTransaction"/> and default settings for CSV file.
        /// </summary>
        /// <param name="connection">Database connection where the data will be inserted. The connection mus be opened.
        /// If there already is running transaction in this connection, it must be specified in
        /// <paramref name="externalTransaction"/>.</param>
        /// <param name="externalTransaction">Transaction in which the bulk insert will be performed.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public MsAccessBulkInsert(OleDbConnection connection, OleDbTransaction externalTransaction)
            : this(connection, externalTransaction, DefaultCodePage, DefaultValueDelimiter)
        {
        }

        /// <summary>
        /// Creates a new instance with database connection <paramref name="connection"/>, transaction
        /// <paramref name="externalTransaction"/> and CSV file code page <paramref name="csvFileCodePage"/>.
        /// </summary>
        /// <param name="connection">Database connection where the data will be inserted. The connection mus be opened.
        /// If there already is running transaction in this connection, it must be specified in
        /// <paramref name="externalTransaction"/>.</param>
        /// <param name="externalTransaction">Transaction in which the bulk insert will be performed.</param>
        /// <param name="csvFileCodePage">Code page for generated CSV file.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public MsAccessBulkInsert(OleDbConnection connection, OleDbTransaction externalTransaction, int csvFileCodePage)
            : this(connection, externalTransaction, csvFileCodePage, DefaultValueDelimiter)
        {
        }

        /// <summary>
        /// Creates a new instance with database connection <paramref name="connection"/>, transaction
        /// <paramref name="externalTransaction"/> and CSV file settings <paramref name="csvFileCodePage"/>
        /// and <paramref name="valueDelimiter"/>.
        /// </summary>
        /// <param name="connection">Database connection where the data will be inserted. The connection mus be opened.
        /// If there already is running transaction in this connection, it must be specified in
        /// <paramref name="externalTransaction"/>.</param>
        /// <param name="externalTransaction">Transaction in which the bulk insert will be performed.</param>
        /// <param name="csvFileCodePage">Code page for generated CSV file.</param>
        /// <param name="valueDelimiter">Value delimiter for generated CSV file.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public MsAccessBulkInsert(
            OleDbConnection connection,
            OleDbTransaction externalTransaction,
            int csvFileCodePage,
            char valueDelimiter)
        {
            _connection = Check.NotNull(connection, nameof(connection));
            ExternalTransaction = externalTransaction;
            CodePage = csvFileCodePage;
            ValueDelimiter = valueDelimiter;
        }

        #endregion

        #region Common

        /// <summary>
        /// Code page used for CSV file and bulk insert. Default value is 65001 <see cref="Utf8CodePage"/>.
        /// </summary>
        /// <value>Number of code page.</value>
        public int CodePage { get; }

        /// <summary>
        /// Value separator in generated CSV file.
        /// </summary>
        public char ValueDelimiter { get; }

        /// <summary>
        /// External transaction, in which bulk insert is executed.
        /// </summary>
        public OleDbTransaction ExternalTransaction { get; }

        #endregion

        #region IBulkInsert

        /// <summary>
        /// This setting is not used.
        /// </summary>
        public int BatchSize { get; set; } = 0;

        /// <summary>
        /// This setting is not used.
        /// </summary>
        public int BulkInsertTimeout { get; set; }

        /// <summary>
        /// Destination table name in database.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <inheritdoc cref="IBulkInsert.ColumnMappings"/>
        public BulkInsertColumnMappingCollection ColumnMappings { get; } = new BulkInsertColumnMappingCollection();

        /// <inheritdoc/>
        public void Insert(IBulkActionDataReader reader)
        {
            using (var bulkInsertReader = new BulkActionDataReader(reader))
            {
                Insert(bulkInsertReader);
            }
        }

        /// <inheritdoc/>
        public async Task InsertAsync(IBulkActionDataReader reader)
        {
            using (var bulkInsertReader = new BulkActionDataReader(reader))
            {
                await InsertAsync(bulkInsertReader);
            }
        }

        /// <inheritdoc/>
        public void Insert(IDataReader reader) => InsertCoreAsync(reader, useAsync: false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public Task InsertAsync(IDataReader reader) => InsertCoreAsync(reader, useAsync: true);

        private async Task InsertCoreAsync(IDataReader reader, bool useAsync)
        {
            string filePath = null;

            try
            {
                _columns.Clear();
                filePath = CreateDataFile(reader);
                InitBulkInsert(filePath, reader);
                await InsertAsync(filePath, useAsync);
            }
            finally
            {
                if (filePath != null)
                {
                    try
                    {
                        string dataFolder = Path.GetDirectoryName(filePath);
                        if (Directory.Exists(dataFolder))
                        {
                            Directory.Delete(dataFolder, true);
                        }
                    }
                    catch { }
                }
            }
        }

        /// <inheritdoc/>
        public void Insert(DataTable table)
        {
            using (var reader = table.CreateDataReader())
            {
                Insert(reader);
            }
        }

        /// <inheritdoc/>
        public async Task InsertAsync(DataTable table)
        {
            using (var reader = table.CreateDataReader())
            {
                await InsertAsync(reader);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private async Task InsertAsync(string sourceFilePath, bool useAsync)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException(string.Format(Resources.CsvFileDoesNotExist, sourceFilePath), sourceFilePath);
            }
            if ((new FileInfo(sourceFilePath)).Length == 0)
            {
                return;
            }

            if ((ExternalTransaction == null) && _connection.IsOpened())
            {
                _connection.Close();
                _connection.Open();
            }

            using (ConnectionHelper.OpenConnection(_connection))
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = CreateInsertSql(DestinationTableName, sourceFilePath);
                cmd.Transaction = ExternalTransaction;
                if (useAsync)
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                else
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Helpers

        private string CreateDataFile(IDataReader data)
        {
            string dataFilePath;

            using (CsvFileWriter csvWriter = CreateCsvWriter())
            {
                dataFilePath = csvWriter.FilePath;
                if (ColumnMappings.Count == 0)
                {
                    csvWriter.Write(data);
                }
                else
                {
                    InitBulkInsertColumns(data);
                    csvWriter.Write(data, from column in _columns select column.SourceColumnName);
                }
            }

            return dataFilePath;
        }

        private void InitBulkInsertColumns(IDataReader data)
        {
            for (int i = 0; i < ColumnMappings.Count; i++)
            {
                BulkInsertColumnMapping mapping = ColumnMappings[i];
                if (!string.IsNullOrWhiteSpace(mapping.SourceName))
                {
                    try
                    {
                        data.GetOrdinal(mapping.SourceName); // Throws some exception if name is invalid.
                        _columns.Add(new MsAccessBulkInsertColumn(mapping.SourceName, mapping));
                    }
                    catch (Exception ex)
                    {
                        ThrowExceptionInvalidSourceColumnMapping(i, null, mapping.SourceName, ex);
                    }
                }
                else if (mapping.SourceOrdinal >= 0)
                {
                    if (mapping.SourceOrdinal >= data.FieldCount)
                    {
                        ThrowExceptionInvalidSourceColumnMapping(i, mapping.SourceOrdinal);
                    }
                    _columns.Add(new MsAccessBulkInsertColumn(data.GetName(mapping.SourceOrdinal), mapping));
                }
                else
                {
                    ThrowExceptionInvalidSourceColumnMapping(i);
                }
            }
        }

        private CsvFileWriter CreateCsvWriter()
        {
            string dataFile = Path.Combine(CreateDataFolder(), "data.csv");
            if (CodePage == Utf8CodePage)
            {
                return new CsvFileWriter(dataFile, new UTF8Encoding(false), false);
            }
            return new CsvFileWriter(dataFile, CodePage, false);
        }

        private string CreateDataFolder()
        {
            const string mainFolderName = "KrosBulkInsert";
            string folder = Path.Combine(Path.GetTempPath(), mainFolderName, Guid.NewGuid().ToString());
            while (Directory.Exists(folder))
            {
                folder = Path.Combine(Path.GetTempPath(), mainFolderName, Guid.NewGuid().ToString());
            }
            Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// Creates SQL command for inserting data into table <paramref name="tableName"/>
        /// from file <paramref name="sourceFilePath"/>.
        /// </summary>
        /// <param name="tableName">Destination table.</param>
        /// <param name="sourceFilePath">Source file.</param>
        /// <returns>Insert statement.</returns>
        private string CreateInsertSql(string tableName, string sourceFilePath)
        {
            StringBuilder sql = new StringBuilder(2000);

            sql.AppendFormat("INSERT INTO {0} (", tableName);
            foreach (var column in _columns)
            {
                sql.AppendFormat("[{0}], ", column.DestinationColumnName);
            }
            sql.Length -= 2; // Removes last comma and space.
            sql.AppendLine(")");

            sql.Append("SELECT ");
            int i = 0;
            foreach (var column in _columns)
            {
                i += 1;
                if (column.ColumnType == BulkInsertColumnType.Text)
                {
                    sql.AppendFormat("IIF(F{0} IS NULL, '', F{0}) AS [{1}], ", i, column.DestinationColumnName);
                }
                else
                {
                    sql.AppendFormat("F{0} AS [{1}], ", i, column.DestinationColumnName);
                }
            }
            sql.Length -= 2; // Removes last comma and space.
            sql.AppendLine();
            sql.AppendFormat("FROM [Text;Database={0}].[{1}]",
                Path.GetDirectoryName(sourceFilePath), Path.GetFileName(sourceFilePath));

            return sql.ToString();
        }

        private string SqlCodePage
        {
            get
            {
                if (CodePage == AnsiCodePage)
                {
                    return "ANSI";
                }
                else if (CodePage == OemCodePage)
                {
                    return "OEM";
                }
                return CodePage.ToString();
            }
        }

        private void InitBulkInsert(string dataFilePath, IDataReader data)
        {
            using (StreamWriter schemaFile = InitSchemaFile(dataFilePath))
            {
                if (ColumnMappings.Count == 0)
                {
                    InitImplicitSchemaColumns(schemaFile, data);
                }
                else
                {
                    InitExplicitSchemaColumns(schemaFile);
                }
            }
        }

        private StreamWriter InitSchemaFile(string dataFilePath)
        {
            string dataFolder = Path.GetDirectoryName(dataFilePath);
            string fileName = Path.GetFileName(dataFilePath);

            StreamWriter writer = new StreamWriter(Path.Combine(dataFolder, "schema.ini"), false, Encoding.ASCII);

            writer.WriteLine($"[{fileName}]");
            writer.WriteLine($"Format=Delimited({ValueDelimiter})");
            writer.WriteLine($"CharacterSet={SqlCodePage}");
            writer.WriteLine("MaxScanRows=25");
            writer.WriteLine("ColNameHeader=False");
            writer.WriteLine("DecimalSymbol=.");
            writer.WriteLine("DateTimeFormat=yyyy-mm-dd hh:nn:ss");

            return writer;
        }

        private void InitImplicitSchemaColumns(StreamWriter schemaFile, IDataReader data)
        {
            var schemaLoader = new MsAccessSchemaLoader();
            TableSchema tableSchema = schemaLoader.LoadTableSchema(_connection, DestinationTableName);
            for (int columnNumber = 0; columnNumber < data.FieldCount; columnNumber++)
            {
                string columnName = data.GetName(columnNumber);
                if (tableSchema.Columns.Contains(columnName))
                {
                    MsAccessColumnSchema column = (MsAccessColumnSchema)tableSchema.Columns[columnName];
                    _columns.Add(new MsAccessBulkInsertColumn(columnName, column.Name, GetBulkInsertColumnType(column)));
                    WriteSchemaColumnInfo(schemaFile, columnNumber + 1, column);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.BulkInsertColumnDoesNotExistInDestination, columnName));
                }
            }
        }

        private void InitExplicitSchemaColumns(StreamWriter schemaFile)
        {
            var schemaLoader = new MsAccessSchemaLoader();
            TableSchema tableSchema = schemaLoader.LoadTableSchema(_connection, DestinationTableName);
            for (int columnNumber = 0; columnNumber < ColumnMappings.Count; columnNumber++)
            {
                MsAccessColumnSchema databaseColumn = null;
                MsAccessBulkInsertColumn bulkInsertColumn = _columns[columnNumber];
                BulkInsertColumnMapping mapping = bulkInsertColumn.Mapping;
                if (!string.IsNullOrWhiteSpace(mapping.DestinationName))
                {
                    if (tableSchema.Columns.Contains(mapping.DestinationName))
                    {
                        databaseColumn = (MsAccessColumnSchema)tableSchema.Columns[mapping.DestinationName];
                    }
                    else
                    {
                        ThrowExceptionInvalidDestinationColumnMapping(columnNumber, null, mapping.DestinationName);
                    }
                }
                else if (mapping.DestinationOrdinal >= 0)
                {
                    if (mapping.DestinationOrdinal >= tableSchema.Columns.Count)
                    {
                        ThrowExceptionInvalidDestinationColumnMapping(columnNumber, mapping.DestinationOrdinal);
                    }
                    else
                    {
                        databaseColumn = (MsAccessColumnSchema)tableSchema.Columns[mapping.DestinationOrdinal];
                    }
                }
                else
                {
                    ThrowExceptionInvalidDestinationColumnMapping(columnNumber);
                }
                bulkInsertColumn.DestinationColumnName = databaseColumn.Name;
                bulkInsertColumn.ColumnType = GetBulkInsertColumnType(databaseColumn);
                WriteSchemaColumnInfo(schemaFile, columnNumber + 1, databaseColumn);
            }
        }

        private void WriteSchemaColumnInfo(StreamWriter schemaFile, int columnNumber, MsAccessColumnSchema column)
        {
            schemaFile.WriteLine($"Col{columnNumber}=F{columnNumber} {GetColumnDataType(column)}");
        }

        // Returns column type for bulk insert according to the data column's data type.
        private static BulkInsertColumnType GetBulkInsertColumnType(MsAccessColumnSchema column)
        {
            if ((column.OleDbType == OleDbType.VarChar) ||
                (column.OleDbType == OleDbType.VarWChar) ||
                (column.OleDbType == OleDbType.LongVarChar) ||
                (column.OleDbType == OleDbType.LongVarWChar) ||
                (column.OleDbType == OleDbType.Char) ||
                (column.OleDbType == OleDbType.WChar))
            {
                return BulkInsertColumnType.Text;
            }
            return BulkInsertColumnType.Undefined;
        }

        private string GetColumnDataType(MsAccessColumnSchema column)
        {
            switch (column.OleDbType)
            {
                case OleDbType.Boolean:
                    return "Bit";

                case OleDbType.TinyInt:
                case OleDbType.UnsignedTinyInt:
                    return "Byte";

                case OleDbType.SmallInt:
                case OleDbType.UnsignedSmallInt:
                    return "Short";

                case OleDbType.Integer:
                case OleDbType.BigInt:
                case OleDbType.UnsignedInt:
                case OleDbType.UnsignedBigInt:
                    return "Long";

                case OleDbType.Decimal:
                case OleDbType.Numeric:
                    return "Decimal";

                case OleDbType.Currency:
                    return "Currency";

                case OleDbType.Single:
                    return "Single";

                case OleDbType.Double:
                    return "Double";

                case OleDbType.Date:
                    return "DateTime";

                case OleDbType.Guid:
                    return "Text";

                case OleDbType.Char:
                case OleDbType.WChar:
                case OleDbType.VarChar:
                case OleDbType.VarWChar:
                case OleDbType.LongVarWChar:
                case OleDbType.LongVarChar:
                    if (column.Size > 0)
                    {
                        return $"Text Width {column.Size}";
                    }
                    else
                    {
                        return "Memo";
                    }

                default:
                    throw new InvalidOperationException(string.Format(Resources.UnsupportedBulkInsertDataType, column.OleDbType));
            }
        }

        private void ThrowExceptionInvalidSourceColumnMapping(
            int columnMappingIndex,
            int? mappingOrdinal = null,
            string mappingName = null,
            Exception innerException = null)
        {
            string exceptionDetail;
            if (mappingOrdinal.HasValue)
            {
                exceptionDetail = string.Format(Resources.InvalidIndexInSourceColumnMapping, mappingOrdinal.Value);
            }
            else if (mappingName != null)
            {
                exceptionDetail = string.Format(Resources.InvalidNameInSourceColumnMapping, mappingName);
            }
            else
            {
                exceptionDetail = Resources.SourceColumnMappingNotSet;
            }

            throw new InvalidOperationException(
                string.Format(Resources.BulkInsertInvalidSourceColumnMapping, columnMappingIndex) + " " + exceptionDetail,
                innerException);
        }

        private void ThrowExceptionInvalidDestinationColumnMapping(
            int columnMappingIndex,
            int? mappingOrdinal = null,
            string mappingName = null)
        {
            string exceptionDetail;
            if (mappingOrdinal.HasValue)
            {
                exceptionDetail = string.Format(Resources.InvalidIndexInDestinationColumnMapping,
                    DestinationTableName, mappingOrdinal.Value);
            }
            else if (mappingName != null)
            {
                exceptionDetail = string.Format(Resources.InvalidNameInDestinationColumnMapping,
                    DestinationTableName, mappingName);
            }
            else
            {
                exceptionDetail = Resources.DestinationColumnMappingNotSet;
            }

            throw new InvalidOperationException(
                string.Format(Resources.BulkInsertInvalidDestinationColumnMapping, columnMappingIndex) + " " + exceptionDetail);
        }

        #endregion

        #region IDisposable

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Dispose()
        {
            if (_disposeOfConnection && (_connection != null))
            {
                _connection.Dispose();
                _connection = null;
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
