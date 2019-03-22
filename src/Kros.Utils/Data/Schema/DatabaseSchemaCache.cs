using Kros.Utils;
using Kros.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Implementation of <see cref="IDatabaseSchemaCache"/>. It allows to load database schemas after adding appropriate
    /// loaders (<see cref="AddSchemaLoader(IDatabaseSchemaLoader, ISchemaCacheKeyGenerator)"/>). Loading of a database
    /// schema is quite slow, that's why loaded schemas are cached. On next request for the same schema, it is not
    /// loaded from database, but returned from cache.
    /// </summary>
    /// <remarks>
    /// <para>It is necessary to initialize <c>DatabaseSchemaCache</c> with loaders (<see cref="IDatabaseSchemaLoader"/>)
    /// and every loader must have its cache key generator (<see cref="ISchemaCacheKeyGenerator"/>). Different cache key
    /// generators should generate different keys.</para>
    /// <para>There is a property <see cref="Default">DatabaseSchemaCache.Default</see> intended for simple static use,
    /// so it is not necessary to create own instance.</para>
    /// </remarks>
    public class DatabaseSchemaCache
        : IDatabaseSchemaCache
    {
        #region Nested types

        private class LoaderInfo
        {
            public LoaderInfo(IDatabaseSchemaLoader loader, ISchemaCacheKeyGenerator keyGenerator)
            {
                Loader = loader;
                KeyGenerator = keyGenerator;
            }
            public IDatabaseSchemaLoader Loader;
            public ISchemaCacheKeyGenerator KeyGenerator;
        }

        #endregion

        #region Static

        /// <summary>
        /// Instance of <c>DatabaseSchemaCache</c> intended for simple static use. By default, it contains a loader
        /// for Microsoft SQL Server (<see cref="SqlServer.SqlServerSchemaLoader"/>).
        /// </summary>
        public static DatabaseSchemaCache Default { get; } = InitDefault();

        private static DatabaseSchemaCache InitDefault()
        {
            var cache = new DatabaseSchemaCache();
            cache.AddSchemaLoader(new SqlServer.SqlServerSchemaLoader(), new SqlServer.SqlServerCacheKeyGenerator());

            return cache;
        }

        #endregion

        #region Private fields

        private readonly List<LoaderInfo> _loaders = new List<LoaderInfo>();
        private readonly ConcurrentDictionary<string, DatabaseSchema> _cache =
            new ConcurrentDictionary<string, DatabaseSchema>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region IDatabaseSchemaCache

        /// <inheritdoc cref="IDatabaseSchemaCache.GetSchema(object)"/>
        /// <exception cref="InvalidOperationException">The cache does not contain a loader for database type
        /// specified by <paramref name="connection"/>.</exception>
        public DatabaseSchema GetSchema(object connection)
        {
            LoaderInfo linfo = GetLoaderInfo(connection);
            return _cache.GetOrAdd(linfo.KeyGenerator.GenerateKey(connection), (k) => linfo.Loader.LoadSchema(connection));
        }

        /// <inheritdoc cref="IDatabaseSchemaCache.ClearSchema(object)"/>
        /// <exception cref="InvalidOperationException">The cache does not contain a loader for database type
        /// specified by <paramref name="connection"/>.</exception>
        public void ClearSchema(object connection)
        {
            LoaderInfo linfo = GetLoaderInfo(connection);
            _cache.TryRemove(linfo.KeyGenerator.GenerateKey(connection), out DatabaseSchema schema);
        }

        /// <inheritdoc cref="IDatabaseSchemaCache.ClearAllSchemas"/>
        public void ClearAllSchemas()
        {
            _cache.Clear();
        }

        /// <inheritdoc cref="IDatabaseSchemaCache.RefreshSchema(object)"/>
        /// <exception cref="InvalidOperationException">The cache does not contain a loader for database type
        /// specified by <paramref name="connection"/>.</exception>
        public DatabaseSchema RefreshSchema(object connection)
        {
            LoaderInfo linfo = GetLoaderInfo(connection);
            DatabaseSchema schema = linfo.Loader.LoadSchema(connection);
            _cache.AddOrUpdate(linfo.KeyGenerator.GenerateKey(connection), schema, (k, v) => schema);
            return schema;
        }

        #endregion

        #region Loaders

        /// <summary>
        /// Adds <paramref name="loader"/> together with its cache key generator <paramref name="keyGenerator"/> to the
        /// inner loaders list.
        /// </summary>
        /// <param name="loader">Database schema loader.</param>
        /// <param name="keyGenerator">Schema cache key generator for <paramref name="loader"/>.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="loader"/> or <paramref name="keyGenerator"/>
        /// is <see langword="null"/>.</exception>
        public void AddSchemaLoader(IDatabaseSchemaLoader loader, ISchemaCacheKeyGenerator keyGenerator)
        {
            Check.NotNull(loader, nameof(loader));
            Check.NotNull(keyGenerator, nameof(keyGenerator));
            _loaders.Add(new LoaderInfo(loader, keyGenerator));
        }

        /// <summary>
        /// Removes specified <paramref name="loader"/> from inner loaders list.
        /// </summary>
        /// <param name="loader">Database schema loader to be removed.</param>
        public void RemoveSchemaLoader(IDatabaseSchemaLoader loader)
        {
            _loaders.Remove(_loaders.FirstOrDefault((linfo) => linfo.Loader == loader));
        }

        /// <summary>
        /// Removes all database schema loaders.
        /// </summary>
        public void ClearSchemaLoaders()
        {
            _loaders.Clear();
        }

        #endregion

        #region Helpers

        private LoaderInfo GetLoaderInfo(object connection)
        {
            LoaderInfo linfo = _loaders.FirstOrDefault((tmpLoader) => tmpLoader.Loader.SupportsConnectionType(connection));
            if (linfo == null)
            {
                throw new InvalidOperationException(
                    string.Format(Resources.UnsupportedConnectionType, connection.GetType().FullName));
            }
            return linfo;
        }

        #endregion
    }
}
