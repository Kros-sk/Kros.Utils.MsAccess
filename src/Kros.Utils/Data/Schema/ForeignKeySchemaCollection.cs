using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;

namespace Kros.Data.Schema
{
    /// <summary>
    /// List of foreign keys for table <see cref="TableSchema"/>.
    /// </summary>
    /// <remarks>To the foreign keys added to this list is automatically set their <see cref="ForeignKeySchema.Table"/>.
    /// Foreign key can belong only to one table.</remarks>
    public class ForeignKeySchemaCollection
        : System.Collections.ObjectModel.KeyedCollection<string, ForeignKeySchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new foreign key list for <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to which foreign key list belongs.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="table"/> is <see langword="null"/>.</exception>
        public ForeignKeySchemaCollection(TableSchema table)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Table = Check.NotNull(table, nameof(table));
        }

        #endregion

        #region Common

        /// <summary>
        /// The table to which belongs this <c>ForeignKeySchemaCollection</c>.
        /// </summary>
        TableSchema Table { get; }

        /// <summary>
        /// Creates a definition of foreign key with the <paramref name="name"/> and adds it to the list.
        /// Column <paramref name="primaryKeyTableColumn"/> in parent table <paramref name="primaryKeyTableName"/> is
        /// referenced in column <paramref name="foreignKeyTableColumn"/> of child table <paramref name="foreignKeyTableName"/>.
        /// </summary>
        /// <param name="name">Name of the foreign key.</param>
        /// <param name="primaryKeyTableName"><inheritdoc cref="ForeignKeySchema.PrimaryKeyTableName" select="summary"/>.</param>
        /// <param name="primaryKeyTableColumn">Column name in primary key table.</param>
        /// <param name="foreignKeyTableName"><inheritdoc cref="ForeignKeySchema.ForeignKeyTableName" select="summary"/>.</param>
        /// <param name="foreignKeyTableColumn">Column name in foreign key table.</param>
        /// <returns>Created foreign key.</returns>
        /// <exception cref="ArgumentNullException">Value of any parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of any parameter is empty string, or string containing only
        /// whitespace characters.</exception>
        public ForeignKeySchema Add(
            string name,
            string primaryKeyTableName,
            string primaryKeyTableColumn,
            string foreignKeyTableName,
            string foreignKeyTableColumn)
        {
            ForeignKeySchema foreignKey = new ForeignKeySchema(
                name,
                primaryKeyTableName, primaryKeyTableColumn,
                foreignKeyTableName, foreignKeyTableColumn);
            Add(foreignKey);
            return foreignKey;
        }

        /// <summary>
        /// Creates a definition of foreign key with the <paramref name="name"/> and adds it to the list.
        /// Columns <paramref name="primaryKeyTableColumns"/> in parent table <paramref name="primaryKeyTableName"/> are
        /// referenced in columns <paramref name="foreignKeyTableColumns"/> of child table <paramref name="foreignKeyTableName"/>.
        /// </summary>
        /// <param name="name">Name of the foreign key.</param>
        /// <param name="primaryKeyTableName"><inheritdoc cref="ForeignKeySchema.PrimaryKeyTableName" select="summary"/>.</param>
        /// <param name="primaryKeyTableColumns">List of columns in parent table.</param>
        /// <param name="foreignKeyTableName"><inheritdoc cref="ForeignKeySchema.ForeignKeyTableName" select="summary"/>.</param>
        /// <param name="foreignKeyTableColumns">List of columns in child table.</param>
        /// <exception cref="ArgumentNullException">Value of any argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="name"/>, <paramref name="primaryKeyTableName"/> or
        /// <paramref name="foreignKeyTableName"/> is empty string, or string containing only whitespace characters.</item>
        /// <item><paramref name="primaryKeyTableColumns"/> or <paramref name="foreignKeyTableColumns"/> is empty
        /// (contains no items).</item>
        /// </list></exception>
        public ForeignKeySchema Add(
            string name,
            string primaryKeyTableName,
            IEnumerable<string> primaryKeyTableColumns,
            string foreignKeyTableName,
            IEnumerable<string> foreignKeyTableColumns)
        {
            ForeignKeySchema foreignKey = new ForeignKeySchema(
                name,
                primaryKeyTableName, primaryKeyTableColumns,
                foreignKeyTableName, foreignKeyTableColumns);
            Add(foreignKey);
            return foreignKey;
        }

        #endregion

        #region KeyedCollection

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override string GetKeyForItem(ForeignKeySchema item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, ForeignKeySchema item)
        {
            if (item.Table == null)
            {
                item.Table = Table;
            }
            else if (item.Table != Table)
            {
                throw new InvalidOperationException(string.Format(Resources.ForeignKeyBelongsToAnotherTable,
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
