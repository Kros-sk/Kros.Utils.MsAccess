namespace Kros.Data.Schema
{
    /// <summary>
    /// Cache key generator for <see cref="DatabaseSchemaCache"/>.
    /// </summary>
    public interface ISchemaCacheKeyGenerator
    {
        /// <summary>
        /// Generates a cache key for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>String, which identifies <paramref name="connection"/>.</returns>
        string GenerateKey(object connection);
    }

    /// <inheritdoc cref="ISchemaCacheKeyGenerator"/>.
    /// <typeparam name="T">Database connection type.</typeparam>
    public interface ISchemaCacheKeyGenerator<T>
        : ISchemaCacheKeyGenerator
    {
        /// <inheritdoc cref="ISchemaCacheKeyGenerator.GenerateKey(object)"/>
        string GenerateKey(T connection);
    }
}
