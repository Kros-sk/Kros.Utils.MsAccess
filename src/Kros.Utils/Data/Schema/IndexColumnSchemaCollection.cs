using Kros.Properties;
using Kros.Utils;
using System;

namespace Kros.Data.Schema
{
    /// <summary>
    /// List of index columns.
    /// </summary>
    /// <remarks>To the columns added to this list is automatically set their <see cref="IndexColumnSchema.Index"/>.
    /// The column can belong only to one index.</remarks>
    public class IndexColumnSchemaCollection
        : System.Collections.ObjectModel.KeyedCollection<string, IndexColumnSchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new list of columns for index <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to which column list belongs.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="index"/> is <see langword="null"/>.</exception>
        public IndexColumnSchemaCollection(IndexSchema index)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Index = Check.NotNull(index, nameof(index));
        }

        #endregion

        #region Common

        /// <summary>
        /// The index to which column list belongs.
        /// </summary>
        public IndexSchema Index { get; }

        /// <summary>
        /// Creates the new index column with name <paramref name="columnName"/> and adds it to the list.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Created column.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="columnName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="columnName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexColumnSchema Add(string columnName)
        {
            IndexColumnSchema indexColumn = new IndexColumnSchema(columnName);
            Add(indexColumn);
            return indexColumn;
        }

        /// <summary>
        /// Creates the new index column with name <paramref name="columnName"/> and sort order <paramref name="order"/>
        /// and adds it to the list.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="order">Column sort order.</param>
        /// <returns>Created column.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="columnName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="columnName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexColumnSchema Add(string columnName, SortOrder order)
        {
            IndexColumnSchema indexColumn = new IndexColumnSchema(columnName, order);
            Add(indexColumn);
            return indexColumn;
        }

        #endregion

        #region KeyedCollection

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override string GetKeyForItem(IndexColumnSchema item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, IndexColumnSchema item)
        {
            if (item.Index == null)
            {
                item.Index = Index;
            }
            else if (item.Index != Index)
            {
                throw new InvalidOperationException(string.Format(Resources.ColumnBelongsToAnotherIndex,
                    item.Name, Index.Name, item.Index.Name));
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < Count)
            {
                base[index].Index = null;
            }
            base.RemoveItem(index);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
