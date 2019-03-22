using Kros.Data.SqlServer;
using Kros.Utils;
using System.Data.Common;
using System.Data.SqlClient;

namespace Kros.Data.BulkActions.SqlServer
{
    /// <summary>
    /// Creates instances of <see cref="IBulkInsert"/> for bulk inserting.
    /// </summary>
    /// <seealso cref="Kros.Data.BulkActions.IBulkActionFactory" />
    public class SqlServerBulkActionFactory : IBulkActionFactory
    {
        private readonly SqlConnection _connection;
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBulkActionFactory"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SqlServerBulkActionFactory(SqlConnection connection)
        {
            _connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBulkActionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlServerBulkActionFactory(string connectionString)
        {
            _connectionString = Check.NotNullOrEmpty(connectionString, nameof(connectionString));
        }

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert() =>
            _connection != null ? new SqlServerBulkInsert(_connection) : new SqlServerBulkInsert(_connectionString);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(DbTransaction externalTransaction) =>
            new SqlServerBulkInsert(_connection, externalTransaction as SqlTransaction);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(SqlBulkCopyOptions options) =>
            _connection != null ?
                new SqlServerBulkInsert(_connection, options) :
                new SqlServerBulkInsert(_connectionString, options);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <param name="options">The options.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(DbTransaction externalTransaction, SqlBulkCopyOptions options) =>
            new SqlServerBulkInsert(_connection, externalTransaction as SqlTransaction, options);

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <returns>
        /// The bulk update.
        /// </returns>
        public IBulkUpdate GetBulkUpdate() =>
            _connection != null ? new SqlServerBulkUpdate(_connection) : new SqlServerBulkUpdate(_connectionString);

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk update.</returns>
        public IBulkUpdate GetBulkUpdate(DbTransaction externalTransaction) =>
            new SqlServerBulkUpdate(_connection, externalTransaction as SqlTransaction);

        /// <summary>
        /// Registers factory methods for creation instances to <see cref="BulkActionFactories"/>.
        /// </summary>
        public static void Register() =>
            BulkActionFactories.Register<SqlConnection>(SqlServerDataHelper.ClientId,
                (conn) => new SqlServerBulkActionFactory(conn as SqlConnection),
                (connString) => new SqlServerBulkActionFactory(connString));
    }
}
