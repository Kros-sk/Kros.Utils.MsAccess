using Kros.Utils;
using Kros.Properties;
using System;

namespace Kros.Data.Schema
{
    /// <summary>
    /// List of indexes for table <see cref="TableSchema"/>.
    /// </summary>
    /// <remarks>To the indexes added to this list is automatically set their <see cref="IndexSchema.Table"/>. Index can
    /// belong only to one table.</remarks>
    public class IndexSchemaCollection
        : System.Collections.ObjectModel.KeyedCollection<string, IndexSchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new index list for <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to which index list belongs.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="table"/> is <see langword="null"/>.</exception>
        public IndexSchemaCollection(TableSchema table)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Table = Check.NotNull(table, nameof(table));
        }

        #endregion

        #region Common

        /// <summary>
        /// The table to which belongs this <c>IndexSchemaCollection</c>.
        /// </summary>
        public TableSchema Table { get; }

        /// <summary>
        /// Creates an instance of index with name <paramref name="indexName"/>. Created index is added to the list.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <returns>Created index schema.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema Add(string indexName)
        {
            IndexSchema index = new IndexSchema(indexName);
            Add(index);
            return index;
        }

        /// <summary>
        /// Creates an instance of index of type <paramref name="indexType"/>, with name <paramref name="indexName"/>.
        /// Created index is added to the list.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="indexType">Type of the index.</param>
        /// <returns>Created index schema.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema Add(string indexName, IndexType indexType)
        {
            IndexSchema index = new IndexSchema(indexName, indexType);
            Add(index);
            return index;
        }

        /// <summary>
        /// Creates an instance of index of type <paramref name="indexType"/>, with name <paramref name="indexName"/> and
        /// setting if the index is <paramref name="clustered"/>. Created index is added to the list.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="indexType">Type of the index.</param>
        /// <param name="clustered">Specifies, if the index is <c>CLUSTERED</c>.</param>
        /// <returns>Created index schema.</returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema Add(string indexName, IndexType indexType, bool clustered)
        {
            IndexSchema index = new IndexSchema(indexName, indexType, clustered);
            Add(index);
            return index;
        }

        #endregion

        #region KeyedCollection

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override string GetKeyForItem(IndexSchema item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, IndexSchema item)
        {
            if (item.Table == null)
            {
                item.Table = Table;
            }
            else if (item.Table != Table)
            {
                throw new InvalidOperationException(string.Format(Resources.IndexBelongsToAnotherTable,
                    item.Name, Table.Name, item.Table.Name));
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < Count)
            {
                base[index].Table = null;
            }
            base.RemoveItem(index);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
