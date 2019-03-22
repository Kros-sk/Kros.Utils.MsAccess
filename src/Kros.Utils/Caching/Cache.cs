using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Kros.Caching
{
    /// <summary>
    /// Class for caching data.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="Kros.Caching.ICache{TKey,TValue}" />
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        #region Private fields

        private Dictionary<TKey, TValue> _cache;
        private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{TKey, TValue}"/> class.
        /// </summary>
        public Cache()
        {
            _cache = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{TKey}" /> implementation to use when comparing keys.</param>
        public Cache(IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(comparer, nameof(comparer));

            _cache = new Dictionary<TKey, TValue>(comparer);
        }

        #endregion

        #region Public Methods

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
        public TValue Get(TKey key, Func<TValue> factory)
        {
            _cacheLock.EnterUpgradeableReadLock();

            try
            {
                TValue ret;
                if (_cache.TryGetValue(key, out ret))
                {
                    return ret;
                }
                else
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        ret = factory();
                        _cache[key] = ret;
                        return ret;
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Gets the count of cached data.
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _cache.Clear();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        #endregion
    }
}
