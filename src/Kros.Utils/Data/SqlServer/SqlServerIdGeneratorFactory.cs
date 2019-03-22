using Kros.Utils;
using System.Data.SqlClient;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// Creates an instances of <see cref="SqlServerIdGenerator"/> for specified database.
    /// </summary>
    /// <seealso cref="SqlServerIdGenerator"/>
    /// <seealso cref="IdGeneratorFactories"/>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public class SqlServerIdGeneratorFactory
        : IIdGeneratorFactory
    {
        private readonly string _connectionString;
        private readonly SqlConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerIdGeneratorFactory"/> class.
        /// </summary>
        /// <param name="connection">Database connection. ID generators create IDs for tables in this database.</param>
        public SqlServerIdGeneratorFactory(SqlConnection connection)
        {
            _connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerIdGeneratorFactory"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string.
        /// ID generators create IDs for tables in this database.</param>
        public SqlServerIdGeneratorFactory(string connectionString)
        {
            _connectionString = Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
        }

        /// <inheritdoc/>
        public IIdGenerator GetGenerator(string tableName) =>
            GetGenerator(tableName, 1);

        /// <inheritdoc/>
        public IIdGenerator GetGenerator(string tableName, int batchSize) =>
            _connection != null ?
                new SqlServerIdGenerator(_connection, tableName, batchSize) :
                new SqlServerIdGenerator(_connectionString, tableName, batchSize);

        /// <summary>
        /// Registers factory methods for creating an instance of factory into <see cref="IdGeneratorFactories"/>.
        /// </summary>
        public static void Register() =>
            IdGeneratorFactories.Register<SqlConnection>(SqlServerDataHelper.ClientId,
                (conn) => new SqlServerIdGeneratorFactory(conn as SqlConnection),
                (connString) => new SqlServerIdGeneratorFactory(connString));
    }
}
