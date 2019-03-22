using Kros.Utils;
using System;
using System.Data.SqlClient;

namespace Kros.Data.Schema.SqlServer
{
    /// <summary>
    /// Cache key generator for Microsoft SQL Server used by <see cref="DatabaseSchemaCache"/>.
    /// </summary>
    public class SqlServerCacheKeyGenerator
        : ISchemaCacheKeyGenerator<SqlConnection>
    {
        /// <summary>
        /// Generates a cache key for <paramref name="connection"/>.
        /// The generated key is a string "<c>SqlServer:SERVER\database</c>".
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>String.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public string GenerateKey(SqlConnection connection)
        {
            Check.NotNull(connection, nameof(connection));
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.ConnectionString);
            return "SqlServer:" + builder.DataSource.ToUpper() + @"\" + builder.InitialCatalog.ToLower();
        }

        /// <inheritdoc cref="GenerateKey(SqlConnection)"/>
        string ISchemaCacheKeyGenerator.GenerateKey(object connection)
        {
            return GenerateKey(connection as SqlConnection);
        }
    }
}
