namespace Kros.Data.Schema
{
    /// <summary>
    /// Interface for loading database schema.
    /// </summary>
    public interface IDatabaseSchemaLoader
    {
        /// <summary>
        /// Checks, if specific loader can load schema from <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns><see langword="true"/> if loader can load schema, <see langword="false"/> otherwise.</returns>
        bool SupportsConnectionType(object connection);

        /// <summary>
        /// Loads whole database schema in <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Schema of whole database.</returns>
        DatabaseSchema LoadSchema(object connection);

        /// <summary>
        /// Loads schema of table <paramref name="tableName"/> from database <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Name of the table to load schema.</param>
        /// <returns>Table schema or <see langword="null"/>, if specified table does not exist.</returns>
        TableSchema LoadTableSchema(object connection, string tableName);
    }

    /// <inheritdoc cref="IDatabaseSchemaLoader"/>
    /// <typeparam name="T">Database connection type which loader works with.</typeparam>
    public interface IDatabaseSchemaLoader<T>
        : IDatabaseSchemaLoader
    {
        /// <inheritdoc cref="IDatabaseSchemaLoader.SupportsConnectionType(object)"/>
        bool SupportsConnectionType(T connection);

        /// <inheritdoc cref="IDatabaseSchemaLoader.LoadSchema(object)"/>
        DatabaseSchema LoadSchema(T connection);

        /// <inheritdoc cref="IDatabaseSchemaLoader.LoadTableSchema(object, string)"/>
        TableSchema LoadTableSchema(T connection, string tableName);
    }
}
