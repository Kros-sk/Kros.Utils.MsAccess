namespace Kros.Data.Schema
{
    /// <summary>
    /// Interface for classes which loads and caches database schema.
    /// </summary>
    /// <remarks>
    /// Loading of a database schema can take some time, so it is good to cache loaded schemas for later use.
    /// </remarks>
    public interface IDatabaseSchemaCache
    {
        /// <summary>
        /// Returns database schema <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">database connection.</param>
        /// <returns>Loaded database schema.</returns>
        DatabaseSchema GetSchema(object connection);

        /// <summary>
        /// Removes cached schema loaded for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        void ClearSchema(object connection);

        /// <summary>
        /// Clears the whole cache (removes all cached schemas).
        /// </summary>
        void ClearAllSchemas();

        /// <summary>
        /// Loads database schema for <paramref name="connection"/>. The schema is loaded directly from database even when
        /// it is already cached. New loaded schema is cached.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Loaded database schema.</returns>
        DatabaseSchema RefreshSchema(object connection);
    }
}
