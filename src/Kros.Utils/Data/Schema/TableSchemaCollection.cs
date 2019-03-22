using Kros.Utils;
using Kros.Properties;
using System;

namespace Kros.Data.Schema
{
    /// <summary>
    /// List of tables for <see cref="DatabaseSchema"/>.
    /// </summary>
    /// <remarks>To the tables added to this list is automatically set their <see cref="TableSchema.Database"/>. The table
    /// can belong only to one database.</remarks>
    public class TableSchemaCollection
        : System.Collections.ObjectModel.KeyedCollection<string, TableSchema>
    {
        #region Constructors

        /// <summary>
        /// Creates a new table list for <paramref name="database"/>.
        /// </summary>
        /// <param name="database">Database to which table belongs.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="database"/> is <see langword="null"/>.</exception>
        public TableSchemaCollection(DatabaseSchema database)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Database = Check.NotNull(database, nameof(database));
        }

        #endregion

        #region Common

        /// <summary>
        /// Database to which table list belongs.
        /// </summary>
        public DatabaseSchema Database { get; }

        /// <summary>
        /// Creates a new <see cref="TableSchema"/> with <paramref name="name"/> and adds it to the list.
        /// </summary>
        /// <param name="name">Name of the created table.</param>
        /// <returns>Created table schema.</returns>
        public TableSchema Add(string name)
        {
            TableSchema schema = new TableSchema(Database, name);
            Add(schema);
            return schema;
        }

        #endregion

        #region KeyedCollection

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override string GetKeyForItem(TableSchema item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, TableSchema item)
        {
            if (item.Database == null)
            {
                item.Database = Database;
            }
            else if (item.Database != Database)
            {
                throw new InvalidOperationException(string.Format(Resources.TableBelongsToAnotherDatabase,
                    item.Name, Database.Name, item.Database.Name));
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < Count)
            {
                base[index].Database = null;
            }
            base.RemoveItem(index);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
