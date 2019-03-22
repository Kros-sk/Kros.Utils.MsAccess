using Kros.Utils;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// The unique ID generator for Microsoft SQL Server.
    /// </summary>
    /// <seealso cref="IdGeneratorFactories" />
    /// <seealso cref="SqlServerIdGeneratorFactory" />
    /// <remarks>In general, the generator should be created using <see cref="SqlServerIdGeneratorFactory"/>.</remarks>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public class SqlServerIdGenerator : IdGeneratorBase
    {
        private const string GetNewIdSpName = "spGetNewId";

        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connectionString"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connectionString">Connection string to the database.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connectionString"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="connectionString"/> is empty string, or string containing only
        /// whitespace characters.</item>
        /// <item>Value of <paramref name="batchSize"/> is less or equal than 0.</item>
        /// </list></exception>
        public SqlServerIdGenerator(string connectionString, string tableName, int batchSize)
            : base(connectionString, tableName, batchSize)
        {
        }

        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connection"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connection"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Value of <paramref name="batchSize"/> is less or equal than 0.</exception>
        public SqlServerIdGenerator(SqlConnection connection, string tableName, int batchSize)
            : base(connection, tableName, batchSize)
        {
        }

        /// <inheritdoc/>
        protected override int GetNewIdFromDbCore()
        {
            using (var cmd = Connection.CreateCommand() as SqlCommand)
            {
                cmd.CommandText = GetNewIdSpName;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", TableName);
                cmd.Parameters.AddWithValue("@NumberOfItems", BatchSize);

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <inheritdoc/>
        protected override DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

        /// <summary>
        /// Returns SQL script for creating stored procedure, which generates IDs.
        /// </summary>
        public static string GetStoredProcedureCreationScript() =>
            ResourceHelper.GetResourceContent("Kros.Resources.SqlIdGeneratorStoredProcedureScript.sql");

        /// <summary>
        /// Returns SQL script for creating table in database for storing IDs.
        /// </summary>
        public static string GetIdStoreTableCreationScript() =>
            ResourceHelper.GetResourceContent("Kros.Resources.SqlIdGeneratorTableScript.sql");

        /// <inheritdoc/>
        public override void InitDatabaseForIdGenerator()
        {
            using (ConnectionHelper.OpenConnection(Connection))
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'IdStore' AND type = 'U')" +
                    Environment.NewLine + GetIdStoreTableCreationScript();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'spGetNewId' AND type = 'P')" +
                    Environment.NewLine + $"EXEC('{GetStoredProcedureCreationScript().Replace("'", "''")}');";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
