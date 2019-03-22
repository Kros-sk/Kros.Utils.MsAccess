using System;

namespace Kros.Caching
{
    /// <summary>
    /// Interface, which describe class for caching data.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Gets the cached value by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory for creating value, if doesn't exist in cahce.</param>
        /// <returns>
        /// Value from Cache.
        /// </returns>
        /// <remarks>
        /// If value key doesn't exist in cache, then factory is use for creating value and value is set to cache.
        /// </remarks>
        TValue Get(TKey key, Func<TValue> factory);

        /// <summary>
        /// Gets the count of cached data.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();
    }
}
