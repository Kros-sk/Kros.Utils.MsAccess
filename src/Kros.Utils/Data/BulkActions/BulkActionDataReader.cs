using Kros.Utils;
using System;
using System.Data;
using System.Collections;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Wrapper, which extends simple <see cref="IBulkActionDataReader"/> to more complex <see cref="IDataReader"/>.
    /// </summary>
    public class BulkActionDataReader : System.Data.Common.DbDataReader
    {
        #region Fields

        private readonly IBulkActionDataReader _reader;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates <see cref="IDataReader"/> over defined <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Input reader.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
        public BulkActionDataReader(IBulkActionDataReader reader)
        {
            Check.NotNull(reader, nameof(reader));
            _reader = reader;
        }

        #endregion

        #region IDataReader

        /// <summary>
        /// Columns count of the data row.
        /// </summary>
        public override int FieldCount => _reader.FieldCount;

        /// <summary>
        /// Returns column name at index <paramref name="i"/>.
        /// </summary>
        /// <param name="i">Index of column.</param>
        /// <returns>Column name.</returns>
        /// <exception cref="IndexOutOfRangeException">Defined index is not between 0 and <see cref="FieldCount"/>.
        /// </exception>
        public override string GetName(int i) => _reader.GetName(i);

        /// <summary>
        /// Returns column index by its name <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <returns>Index of column.</returns>
        public override int GetOrdinal(string name) => _reader.GetOrdinal(name);

        /// <summary>
        /// Returns value of column.
        /// </summary>
        /// <param name="i">Column index.</param>
        /// <returns>Object value of column.</returns>
        /// <exception cref="IndexOutOfRangeException">Defined index is not between 0 and <see cref="FieldCount"/>.
        /// </exception>
        public override object GetValue(int i) => _reader.GetValue(i);

        /// <inheritdoc cref="IBulkActionDataReader.GetString(int)"/>
        public override string GetString(int i) => _reader.GetString(i);

        /// <inheritdoc cref="IBulkActionDataReader.IsDBNull(int)"/>
        public override bool IsDBNull(int i) => _reader.IsDBNull(i);

        /// <summary>
        /// Moves reader to next record.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if next record exists and reader is moved,
        /// <see langword="false"/> if there is no next record.
        /// </returns>
        public override bool Read() => _reader.Read();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        // Close is implemented just because this class inherits from DbDataReader and in .NET Core is bug
        // in SqlBulkCopy implementation. If reader does not inherit from DbDataReader, it breaks on NullReferenceException
        // in some cases.
        // After solving this issue in .NET Core (https://github.com/dotnet/corefx/issues/24638), this inheritance can be removed.
        // Then class will implement only IDataReader and Close method can stay here non-implemented.
        public override void Close() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region NotImplemented

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool GetBoolean(int i) => throw new NotImplementedException();
        public override bool HasRows => throw new NotImplementedException();
        public override bool IsClosed => throw new NotSupportedException();
        public override bool NextResult() => throw new NotImplementedException();
        public override byte GetByte(int i) => throw new NotImplementedException();
        public override DataTable GetSchemaTable() => throw new NotImplementedException();
        public override DateTime GetDateTime(int i) => throw new NotImplementedException();
        public override decimal GetDecimal(int i) => throw new NotImplementedException();
        public override double GetDouble(int i) => throw new NotImplementedException();
        public override float GetFloat(int i) => throw new NotImplementedException();
        public override Guid GetGuid(int i) => throw new NotImplementedException();
        public override char GetChar(int i) => throw new NotImplementedException();
        public override IEnumerator GetEnumerator() => throw new NotImplementedException();
        public override int Depth => throw new NotSupportedException();
        public override int GetInt32(int i) => throw new NotImplementedException();
        public override int GetValues(object[] values) => throw new NotImplementedException();
        public override int RecordsAffected => throw new NotSupportedException();
        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            throw new NotImplementedException();
        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
            throw new NotImplementedException();
        public override long GetInt64(int i) => throw new NotImplementedException();
        public override object this[int i] => throw new NotSupportedException();
        public override object this[string name] => throw new NotSupportedException();
        public override short GetInt16(int i) => throw new NotImplementedException();
        public override string GetDataTypeName(int i) => throw new NotImplementedException();
        public override Type GetFieldType(int i) => throw new NotImplementedException();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
