using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// General extension methods for IEnumerable (<see cref="System.Collections.Generic.IEnumerable{T}" />).
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Perform the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="source">Input enumerable.</param>
        /// <param name="action">Action that perform <paramref name="action"/> on each element of <paramref name="source"/>.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
