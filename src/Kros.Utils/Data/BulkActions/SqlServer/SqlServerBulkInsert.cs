using Kros.Data.Schema;
using Kros.Properties;
using Kros.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions.SqlServer
{
    /// <summary>
    /// Class for fast data inserting into SQL Server.
    /// </summary>
    public class SqlServerBulkInsert : IBulkInsert
    {
        /// <summary>
        /// Default <see cref="SqlBulkCopyOptions"/> for internal instance of <see cref="SqlBulkCopy"/>,
        /// if external transaction is not used.
        /// Value is <c>SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction</c>.
        /// </summary>
        public static SqlBulkCopyOptions DefaultBulkCopyOptions { get; } =
            SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction;

        /// <summary>
        /// Default <see cref="SqlBulkCopyOptions"/> for internal instance of <see cref="SqlBulkCopy"/>,
        /// if external transaction is used.
        /// Value is <c>SqlBulkCopyOptions.TableLock</c>.
        /// </summary>
        public static SqlBulkCopyOptions DefaultBulkCopyOptionsExternalTransaction { get; } = SqlBulkCopyOptions.TableLock;

        #region Private fields

        private SqlConnection _connection;
        private readonly bool _disposeOfConnection = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with database connection <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be inserted, connection has to be opened.
        /// If transaction is running on connection, contructor with defined external transaction must be used.</param>
        public SqlServerBulkInsert(SqlConnection connection)
            : this(connection, null, DefaultBulkCopyOptions)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with database connection <paramref name="connection"/>
        /// and external transaction <paramref name="externalTransaction"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be inserted, connection has to be opened.
        /// If transaction is running on connection, transaction has to be defined in <paramref name="externalTransaction"/>.
        /// </param>
        /// <param name="externalTransaction">External transaction, in which bulk insert is executed.</param>
        public SqlServerBulkInsert(SqlConnection connection, SqlTransaction externalTransaction)
            : this(connection, externalTransaction,
                  externalTransaction == null ? DefaultBulkCopyOptions : DefaultBulkCopyOptionsExternalTransaction)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with database connection <paramref name="connection"/>
        /// and defined options <paramref name="options"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be inserted, connection has to be opened.</param>
        /// <param name="options">Options <see cref="SqlBulkCopyOptions"/>.</param>
        public SqlServerBulkInsert(SqlConnection connection, SqlBulkCopyOptions options)
            : this(connection, null, options)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with database connection <paramref name="connection"/>,
        /// external transaction <paramref name="externalTransaction"/> and defined options <paramref name="options"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be inserted, connection has to be opened.
        /// If transaction is running on connection, transaction has to be defined in <paramref name="externalTransaction"/>.
        /// </param>
        /// <param name="externalTransaction">External transaction, in which bulk insert is executed.</param>
        /// <param name="options">Options <see cref="SqlBulkCopyOptions"/>.</param>
        public SqlServerBulkInsert(SqlConnection connection, SqlTransaction externalTransaction, SqlBulkCopyOptions options)
        {
            Check.NotNull(connection, nameof(connection));
            _connection = connection;
            ExternalTransaction = externalTransaction;
            BulkCopyOptions = options;
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for database connection.</param>
        public SqlServerBulkInsert(string connectionString)
            : this(connectionString, DefaultBulkCopyOptions)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkInsert"/> with <paramref name="connectionString"/>
        /// and defined options <paramref name="options"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for database connection.</param>
        /// <param name="options">Options <see cref="SqlBulkCopyOptions"/>.</param>
        public SqlServerBulkInsert(string connectionString, SqlBulkCopyOptions options)
            : this(new SqlConnection(connectionString), null, options)
        {
            _disposeOfConnection = true;
        }

        #endregion

        #region Common

        /// <summary>
        /// External transaction, in which bulk insert is executed.
        /// </summary>
        public SqlTransaction ExternalTransaction { get; }

        /// <summary>
        /// Options <see cref="BulkCopyOptions"/> for internal instance of <see cref="SqlBulkCopy"/>.
        /// </summary>
        public SqlBulkCopyOptions BulkCopyOptions { get; }

        #endregion

        #region IBulkInsert

        private int _batchSize = 0;

        /// <summary>
        /// Row count for batch sent to database. If 0, batch size is not limited.
        /// </summary>
        /// <exception cref="ArgumentException">Value is negative.</exception>
        public int BatchSize
        {
            get => _batchSize;
            set => _batchSize = Check.GreaterOrEqualThan(value, 0, nameof(value));
        }

        private int _bulkInsertTimeout = 0;

        /// <summary>
        /// Timeout for BulkInsert operation. If 0, duration of operation is not limited.
        /// </summary>
        /// <exception cref="ArgumentException">Value is negative.</exception>
        public int BulkInsertTimeout
        {
            get => _bulkInsertTimeout;
            set => _bulkInsertTimeout = Check.GreaterOrEqualThan(value, 0, nameof(value));
        }

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
            using (ConnectionHelper.OpenConnection(_connection))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection, BulkCopyOptions, ExternalTransaction))
            {
                bulkCopy.DestinationTableName = DestinationTableName;
                bulkCopy.BulkCopyTimeout = BulkInsertTimeout;
                bulkCopy.BatchSize = BatchSize;
                bulkCopy.EnableStreaming = true;
                SetColumnMappings(bulkCopy, reader);
                if (useAsync)
                {
                    await bulkCopy.WriteToServerAsync(reader);
                }
                else
                {
                    bulkCopy.WriteToServer(reader);
                }
            }
        }

        private void SetColumnMappings(SqlBulkCopy bulkCopy, IDataReader reader)
        {
            if (ColumnMappings.Count == 0)
            {
                SetImplicitColumnMappings(bulkCopy, reader);
            }
            else
            {
                SetExplicitColumnMappings(bulkCopy);
            }
        }

        private void SetImplicitColumnMappings(SqlBulkCopy bulkCopy, IDataReader reader)
        {
            var tableSchema = DatabaseSchemaLoader.Default.LoadTableSchema(_connection, DestinationTableName);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string sourceColumn = reader.GetName(i);
                string destinationColumn = sourceColumn;

                if (tableSchema != null)
                {
                    if (tableSchema.Columns.Contains(sourceColumn))
                    {
                        destinationColumn = tableSchema.Columns[sourceColumn].Name;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format(Resources.BulkInsertColumnDoesNotExistInDestination,
                            bulkCopy.DestinationTableName, sourceColumn));
                    }
                }
                bulkCopy.ColumnMappings.Add(sourceColumn, destinationColumn);
            }
        }

        private void SetExplicitColumnMappings(SqlBulkCopy bulkCopy)
        {
            var tableSchema = DatabaseSchemaLoader.Default.LoadTableSchema(_connection, DestinationTableName);

            for (int columnNumber = 0; columnNumber < ColumnMappings.Count; columnNumber++)
            {
                SqlBulkCopyColumnMapping mapping = ConvertColumnMapping(ColumnMappings[columnNumber]);
                if (tableSchema != null)
                {
                    if (!string.IsNullOrWhiteSpace(mapping.DestinationColumn))
                    {
                        if (tableSchema.Columns.Contains(mapping.DestinationColumn))
                        {
                            mapping.DestinationColumn = tableSchema.Columns[mapping.DestinationColumn].Name;
                        }
                        else
                        {
                            ThrowExceptionInvalidDestinationColumnMapping(columnNumber, null, mapping.DestinationColumn);
                        }
                    }
                    else if (mapping.DestinationOrdinal >= 0)
                    {
                        if (mapping.DestinationOrdinal >= tableSchema.Columns.Count)
                        {
                            ThrowExceptionInvalidDestinationColumnMapping(columnNumber, mapping.DestinationOrdinal);
                        }
                    }
                    else
                    {
                        ThrowExceptionInvalidDestinationColumnMapping(columnNumber);
                    }
                }
                bulkCopy.ColumnMappings.Add(mapping);
            }
        }

        private SqlBulkCopyColumnMapping ConvertColumnMapping(BulkInsertColumnMapping mapping)
        {
            if (!string.IsNullOrEmpty(mapping.SourceName) && !string.IsNullOrEmpty(mapping.DestinationName))
            {
                return new SqlBulkCopyColumnMapping(mapping.SourceName, mapping.DestinationName);
            }
            else if (!string.IsNullOrEmpty(mapping.SourceName))
            {
                return new SqlBulkCopyColumnMapping(mapping.SourceName, mapping.DestinationOrdinal);
            }
            else if (!string.IsNullOrEmpty(mapping.DestinationName))
            {
                return new SqlBulkCopyColumnMapping(mapping.SourceOrdinal, mapping.DestinationName);
            }
            return new SqlBulkCopyColumnMapping(mapping.SourceOrdinal, mapping.DestinationOrdinal);
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

        #endregion

        #region IDisposable

        private bool disposedValue = false;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_disposeOfConnection)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
