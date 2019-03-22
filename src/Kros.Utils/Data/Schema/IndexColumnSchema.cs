using Kros.Utils;
using System;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Schema of a column of an index.
    /// </summary>
    public class IndexColumnSchema
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of an index column with <paramref name="name"/>. Column sort <see cref="Order"/> is
        /// <see cref="SortOrder.Ascending"/>.
        /// </summary>
        /// <param name="name">Index column name.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="name"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexColumnSchema(string name)
            : this(name, SortOrder.Ascending)
        {
        }

        /// <summary>
        /// Creates an instance of an index column with <paramref name="name"/> and sort <paramref name="order"/>.
        /// </summary>
        /// <param name="name">Index column name.</param>
        /// <param name="order">Index column sort order.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="name"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexColumnSchema(string name, SortOrder order)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            Order = order;
        }

        #endregion

        #region Common

        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sort order of the column.
        /// </summary>
        public SortOrder Order { get; set; } = SortOrder.Ascending;

        /// <summary>
        /// Index, to which column belongs.
        /// </summary>
        public IndexSchema Index { get; internal set; }

        #endregion
    }
}
