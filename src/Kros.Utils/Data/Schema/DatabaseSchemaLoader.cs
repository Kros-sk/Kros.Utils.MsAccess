using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Helper class for simple loading of database schemas. It contains inner list of different loaders, so it can be used
    /// for loading database schema for different databases. Fresh database schema is loaded for every request,
    /// it means, loaded schemas are not cached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Class is a wrapper for loaders for different databases and so it allows loading of database schema for any known
    /// database type. More loaders can be added by <see cref="DatabaseSchemaLoader.AddSchemaLoader(IDatabaseSchemaLoader)"/>
    /// </para>
    /// <para>
    /// The class is intended for static use, using property <see cref="DatabaseSchemaLoader.Default"/>.
    /// By default, it contains loader for Microsoft SQL Server (<see cref="SqlServer.SqlServerSchemaLoader"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\SchemaExamples.cs" region="SchemaLoader"/>
    /// </example>
    public class DatabaseSchemaLoader
        : IDatabaseSchemaLoader
    {
        #region Static

        /// <summary>
        /// Instance of <c>DatabaseSchemaLoader</c> intended for general use. It contains loader for Microsoft SQL Server
        /// by default (<see cref="SqlServer.SqlServerSchemaLoader">SqlServerSchemaLoader</see>).
        /// </summary>
        public static DatabaseSchemaLoader Default { get; } = InitDefault();

        private static DatabaseSchemaLoader InitDefault()
        {
            DatabaseSchemaLoader loader = new DatabaseSchemaLoader();
            loader.AddSchemaLoader(new SqlServer.SqlServerSchemaLoader());

            return loader;
        }

        #endregion

        #region Fields

        private readonly List<IDatabaseSchemaLoader> _loaders = new List<IDatabaseSchemaLoader>();

        #endregion

        #region Loaders

        /// <summary>
        /// Adds <paramref name="loader"/> to the list of loaders.
        /// </summary>
        /// <param name="loader">Specific database schema loader.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="loader"/> is <see langword="null"/>.</exception>
        public void AddSchemaLoader(IDatabaseSchemaLoader loader)
        {
            Check.NotNull(loader, nameof(loader));
            _loaders.Add(loader);
        }

        /// <summary>
        /// Removes <paramref name="loader"/> from the list of loaders.
        /// </summary>
        /// <param name="loader">Specific database schema loader to be removed from the list.</param>
        public void RemoveSchemaLoader(IDatabaseSchemaLoader loader)
        {
            _loaders.Remove(loader);
        }

        /// <summary>Removes all loaders in the list.</summary>
        public void ClearSchemaLoaders()
        {
            _loaders.Clear();
        }

        #endregion

        #region IDatabaseSchemaLoader

        private IDatabaseSchemaLoader GetLoader(object connection)
        {
            return _loaders.FirstOrDefault((loader) => loader.SupportsConnectionType(connection));
        }

        private IDatabaseSchemaLoader CheckConnectionAndGetLoader(object connection)
        {
            Check.NotNull(connection, nameof(connection));

            IDatabaseSchemaLoader loader = GetLoader(connection);
            if (loader == null)
            {
                throw new ArgumentException(
                    string.Format(Resources.UnsupportedConnectionType, connection.GetType().FullName), nameof(connection));
            }

            return loader;
        }

        /// <summary>
        /// Checks, if database schema from <paramref name="connection"/> can be loaded.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>
        /// <see langword="true"/> if loading schema for <paramref name="connection"/> is supported, <see langword="false"/>
        /// otherwise. Internal list of loader is checked and method returns <see langword="true"/> if any of the loaders
        /// can load schema from <paramref name="connection"/>.
        /// </returns>
        public bool SupportsConnectionType(object connection)
        {
            Check.NotNull(connection, nameof(connection));
            return GetLoader(connection) != null;
        }

        /// <inheritdoc cref="IDatabaseSchemaLoader{T}.LoadSchema(T)"/>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Loader for connection type specified by <paramref name="connection"/> does not exist.
        /// </exception>
        public DatabaseSchema LoadSchema(object connection)
        {
            IDatabaseSchemaLoader loader = CheckConnectionAndGetLoader(connection);
            return loader.LoadSchema(connection);
        }

        /// <inheritdoc cref="IDatabaseSchemaLoader{T}.LoadTableSchema(T, string)"/>
        /// <exception cref="ArgumentNullException">Value of any parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item>Loader for connection type specified by <paramref name="connection"/> does not exist.</item>
        /// <item>Value of <paramref name="tableName"/> is empty string, or string containing only whitespace characters.</item>
        /// </list>
        /// </exception>
        public TableSchema LoadTableSchema(object connection, string tableName)
        {
            Check.NotNullOrWhiteSpace(tableName, nameof(tableName));

            IDatabaseSchemaLoader loader = CheckConnectionAndGetLoader(connection);
            return loader.LoadTableSchema(connection, tableName);
        }

        #endregion
    }
}
