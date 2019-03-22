using Kros.Utils;
using System;
using System.Data.OleDb;

namespace Kros.Data.Schema.MsAccess
{
    /// <summary>
    /// Cache key generator for Microsoft Access used by <see cref="DatabaseSchemaCache"/>.
    /// </summary>
    public class MsAccessCacheKeyGenerator
        : ISchemaCacheKeyGenerator<OleDbConnection>
    {
        /// <summary>
        /// Generates a cache key for <paramref name="connection"/>.
        /// The generated key is a string "<c>MsAccess:database path</c>".
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>String.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public string GenerateKey(OleDbConnection connection)
        {
            Check.NotNull(connection, nameof(connection));
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connection.ConnectionString);
            return "MsAccess:" + builder.DataSource.ToLower();
        }

        /// <inheritdoc cref="GenerateKey(OleDbConnection)"/>
        string ISchemaCacheKeyGenerator.GenerateKey(object connection)
        {
            return GenerateKey(connection as OleDbConnection);
        }
    }
}
