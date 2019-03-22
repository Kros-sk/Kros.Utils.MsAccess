using Kros.Data.MsAccess;
using Kros.Utils;
using System.Data.Common;
using System.Data.OleDb;

namespace Kros.Data.BulkActions.MsAccess
{
    /// <summary>
    /// Creates instances of <see cref="IBulkInsert"/> for bulk inserting.
    /// </summary>
    /// <seealso cref="Kros.Data.BulkActions.IBulkActionFactory" />
    public class MsAccessBulkActionFactory : IBulkActionFactory
    {
        private readonly OleDbConnection _connection;
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsAccessBulkActionFactory"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MsAccessBulkActionFactory(OleDbConnection connection)
        {
            _connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsAccessBulkActionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MsAccessBulkActionFactory(string connectionString)
        {
            _connectionString = Check.NotNullOrEmpty(connectionString, nameof(connectionString));
        }

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert() =>
            _connection != null ? new MsAccessBulkInsert(_connection) : new MsAccessBulkInsert(_connectionString);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(DbTransaction externalTransaction) =>
            new MsAccessBulkInsert(_connection, externalTransaction as OleDbTransaction);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <param name="csvFileCodePage">The CSV file code page.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(DbTransaction externalTransaction, int csvFileCodePage) =>
            new MsAccessBulkInsert(_connection, externalTransaction as OleDbTransaction, csvFileCodePage);

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <param name="csvFileCodePage">The CSV file code page.</param>
        /// <param name="valueDelimiter">The value delimiter.</param>
        /// <returns>The bulk insert.</returns>
        public IBulkInsert GetBulkInsert(DbTransaction externalTransaction, int csvFileCodePage, char valueDelimiter) =>
            new MsAccessBulkInsert(_connection, externalTransaction as OleDbTransaction, csvFileCodePage, valueDelimiter);

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <returns>
        /// The bulk update.
        /// </returns>
        public IBulkUpdate GetBulkUpdate() =>
            _connection != null ? new MsAccessBulkUpdate(_connection): new MsAccessBulkUpdate(_connectionString);

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk update.</returns>
        public IBulkUpdate GetBulkUpdate(DbTransaction externalTransaction) =>
            new MsAccessBulkUpdate(_connection, externalTransaction as OleDbTransaction);

        /// <summary>
        /// Registers factory methods for creation instances to <see cref="BulkActionFactories"/>.
        /// </summary>
        public static void Register() =>
            BulkActionFactories.Register<OleDbConnection>(MsAccessDataHelper.ClientId,
                (conn) => new MsAccessBulkActionFactory(conn as OleDbConnection),
                (connString) => new MsAccessBulkActionFactory(connString));
    }
}
