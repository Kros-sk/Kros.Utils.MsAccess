using Kros.Utils;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions.SqlServer
{
    /// <summary>
    /// Class for fast data update for SQL Server.
    /// </summary>
    /// <example>
    ///   <code source="..\..\..\..\Documentation\Examples\Kros.Utils\BulkUpdateExamples.cs" title="Bulk update" region="BulkUpdate" language="cs" />
    /// </example>
    public class SqlServerBulkUpdate : BulkUpdateBase
    {
        #region Constructors

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with database connection <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be updated, connection has to be opened.
        /// If transaction is running on connection, contructor with defined external transaction has to be used.</param>
        public SqlServerBulkUpdate(SqlConnection connection)
            : this(connection, null)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with database connection <paramref name="connection"/>,
        /// and external transaction <paramref name="externalTransaction"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be updated, connection has to be opened.
        /// If transaction is running on connection, transaction has to be defined in <paramref name="externalTransaction"/>.
        /// </param>
        /// <param name="externalTransaction">External transaction, in which bulk update is executed.</param>
        public SqlServerBulkUpdate(SqlConnection connection, SqlTransaction externalTransaction)
        {
            _connection = Check.NotNull(connection, nameof(connection));

            ExternalTransaction = externalTransaction;
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for database connection.</param>
        public SqlServerBulkUpdate(string connectionString)
            : this(new SqlConnection(connectionString), null)
        {
            _disposeOfConnection = true;
        }

        #endregion

        #region BulkUpdateBase Members

        /// <inheritdoc/>
        protected override IBulkInsert CreateBulkInsert()
        {
            return new SqlServerBulkInsert((SqlConnection)_connection, (SqlTransaction)ExternalTransaction);
        }

        /// <inheritdoc/>
        protected override void InvokeAction(string tempTableName)
        {
            TempTableAction?.Invoke(_connection, ExternalTransaction, tempTableName);
        }

        /// <inheritdoc/>
        protected override string GetTempTableName() => $"{PrefixTempTable}{DestinationTableName}_{Guid.NewGuid()}";

        /// <inheritdoc/>
        protected override void CreateTempTable(IDataReader reader, string tempTableName)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Transaction = ExternalTransaction;
                cmd.CommandText = $"SELECT {GetColumnNamesForTempTable(reader)} INTO [{tempTableName}] " +
                                  $"FROM [{DestinationTableName}] " +
                                  $"WHERE (1 = 2)";
                cmd.ExecuteNonQuery();
            }
        }

        /// <inheritdoc/>
        protected override IDbCommand CreateCommandForPrimaryKey()
        {
            var ret = _connection.CreateCommand();

            ret.Transaction = ExternalTransaction;

            return ret;
        }

        /// <inheritdoc/>
        protected async override Task UpdateDestinationTableAsync(IDataReader reader, string tempTableName, bool useAsync)
        {
            using (var cmd = _connection.CreateCommand())
            {
                var innerJoin = $"[{DestinationTableName}].[{PrimaryKeyColumn}] = [{tempTableName}].[{PrimaryKeyColumn}]";

                cmd.Transaction = ExternalTransaction;
                cmd.CommandText = $"UPDATE [{DestinationTableName}] " +
                                  $"SET {GetUpdateColumnNames(reader, tempTableName)} " +
                                  $"FROM [{DestinationTableName}] INNER JOIN [{tempTableName}] " +
                                                                $"ON ({innerJoin})";

                await ExecuteNonQueryAsync(useAsync, cmd);
            }
        }

        /// <inheritdoc/>
        protected async override Task DoneTempTableAsync(string tempTableName, bool useAsync)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Transaction = ExternalTransaction;
                cmd.CommandText = $"DROP TABLE [{tempTableName}]";

                await ExecuteNonQueryAsync(useAsync, cmd);
            }
        }

        private static async Task ExecuteNonQueryAsync(bool useAsync, IDbCommand cmd)
        {
            if (useAsync)
            {
                await (cmd as DbCommand).ExecuteNonQueryAsync();
            }
            else
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
