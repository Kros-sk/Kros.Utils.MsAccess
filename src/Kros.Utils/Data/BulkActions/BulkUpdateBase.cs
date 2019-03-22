using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Common base class for BulkUpdate.
    /// </summary>
    public abstract class BulkUpdateBase : IBulkUpdate
    {
        #region Constants

        /// <summary>
        /// Temporary table prefix.
        /// </summary>
        protected const char PrefixTempTable = '#';

        #endregion

        #region Private fields

        /// <summary>
        /// Connection.
        /// </summary>
        protected IDbConnection _connection;

        /// <summary>
        /// <see langword="true"/> if dispose of the connection is necessary, otherwise <see langword="false"/>.
        /// </summary>
        protected bool _disposeOfConnection = false;

        #endregion

        #region Properties

        /// <summary>
        /// External transaction in which the operation is executed.
        /// </summary>
        public IDbTransaction ExternalTransaction { get; protected set; }

        #endregion

        #region IBulkUpdate Members

        /// <summary>
        /// Destination table name in database.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <inheritdoc/>
        public Action<IDbConnection, IDbTransaction, string> TempTableAction { get; set; }

        /// <summary>
        /// Primary key.
        /// </summary>
        public string PrimaryKeyColumn { get; set; }

        /// <inheritdoc/>
        public void Update(IBulkActionDataReader reader)
        {
            using (var bulkUpdateReader = new BulkActionDataReader(reader))
            {
                Update(bulkUpdateReader);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(IBulkActionDataReader reader)
        {
            using (var bulkUpdateReader = new BulkActionDataReader(reader))
            {
                await UpdateAsync(bulkUpdateReader);
            }
        }

        /// <inheritdoc/>
        public void Update(IDataReader reader) => UpdateCoreAsync(reader, useAsync: false).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public Task UpdateAsync(IDataReader reader) => UpdateCoreAsync(reader, useAsync: true);

        private async Task UpdateCoreAsync(IDataReader reader, bool useAsync)
        {
            using (ConnectionHelper.OpenConnection(_connection))
            {
                var tempTableName = CreateTempTable(reader);

                await InsertIntoTempTableAsync(reader, tempTableName, useAsync);
                InvokeAction(tempTableName);
                await UpdateDestinationTableAsync(reader, tempTableName, useAsync);
                await DoneTempTableAsync(tempTableName, useAsync);
            }
        }

        ///<inheritdoc/>
        public void Update(DataTable table)
        {
            using (var reader = table.CreateDataReader())
            {
                Update(reader);
            }
        }

        ///<inheritdoc/>
        public async Task UpdateAsync(DataTable table)
        {
            using (var reader = table.CreateDataReader())
            {
                await UpdateAsync(reader);
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Creates BulkInsert.
        /// </summary>
        /// <returns>Bulk insert.</returns>
        protected abstract IBulkInsert CreateBulkInsert();

        /// <summary>
        /// Invokes action in temporary database.
        /// </summary>
        /// <param name="tempTableName">Name of temporary table.</param>
        protected abstract void InvokeAction(string tempTableName);

        /// <summary>
        /// Returns name of temporary table.
        /// </summary>
        protected abstract string GetTempTableName();

        /// <summary>
        /// Creates temporary table by <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Reader for accessing data.</param>
        /// <param name="tempTableName">Name of temporary table.</param>
        protected abstract void CreateTempTable(IDataReader reader, string tempTableName);

        /// <summary>
        /// Returns formatted name of temporary table for BulkInsert.
        /// </summary>
        /// <param name="name">Temporary table name.</param>
        protected virtual string GetTempTableNameForBulkInsert(string name) => $"[{name}]";

        /// <summary>
        /// Returns command for creating primary key.
        /// </summary>
        protected abstract IDbCommand CreateCommandForPrimaryKey();

        /// <summary>
        /// Executes update on destination table.
        /// </summary>
        /// <param name="reader">Reader for accesing data.</param>
        /// <param name="tempTableName">Temporary table name.</param>
        /// <param name="useAsync"><see langword="true"/> if action can by executed asynchronously.</param>
        protected abstract Task UpdateDestinationTableAsync(IDataReader reader, string tempTableName, bool useAsync);

        /// <summary>
        /// Ends work with temporary table.
        /// </summary>
        /// <param name="tempTableName">Temporary table name.</param>
        /// <param name="useAsync"><see langword="true"/> if action can by executed asynchronously.</param>
        protected virtual Task DoneTempTableAsync(string tempTableName, bool useAsync) => Task.CompletedTask;

        #endregion

        #region Helpers

        private string CreateTempTable(IDataReader reader)
        {
            var tempTableName = GetTempTableName();

            CreateTempTable(reader, tempTableName);
            CreatePrimaryKey(tempTableName);

            return tempTableName;
        }

        private void CreatePrimaryKey(string tempTableName)
        {
            using (var cmd = CreateCommandForPrimaryKey())
            {
                cmd.CommandText = $"ALTER TABLE [{tempTableName}] " +
                                  $"ADD CONSTRAINT [PK_{tempTableName.Trim(PrefixTempTable)}] " +
                                  $"PRIMARY KEY NONCLUSTERED ({PrimaryKeyColumn})";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// List of temporary table columns.
        /// </summary>
        /// <param name="reader">Reader for accesing data.</param>
        protected string GetColumnNamesForTempTable(IDataReader reader)
        {
            var ret = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                ret.AppendFormat("[{0}], ", reader.GetName(i));
            }

            ret.Length -= 2;

            return ret.ToString();
        }

        private async Task InsertIntoTempTableAsync(IDataReader reader, string tempTableName, bool useAsync)
        {
            using (var bulkInsert = CreateBulkInsert())
            {
                bulkInsert.DestinationTableName = GetTempTableNameForBulkInsert(tempTableName);
                if (useAsync)
                {
                    await bulkInsert.InsertAsync(reader);
                }
                else
                {
                    bulkInsert.Insert(reader);
                }
            }
        }

        /// <summary>
        /// List of temporary table columns.
        /// </summary>
        /// <param name="reader">Reader for accesing data.</param>
        /// <param name="tempTableName">Temporary table name.</param>
        /// <returns></returns>
        protected string GetUpdateColumnNames(IDataReader reader, string tempTableName)
        {
            var ret = new StringBuilder();
            var columnName = string.Empty;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnName = reader.GetName(i);

                if (!PrimaryKeyColumn.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    ret.AppendFormat("[{0}].[{1}] = [{2}].[{1}], ", DestinationTableName, columnName, tempTableName);
                }
            }

            ret.Length -= 2;

            return ret.ToString();
        }

        #endregion

        #region IDisposable

        private bool _disposedValue = false;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && _disposeOfConnection)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
                _disposedValue = true;
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
