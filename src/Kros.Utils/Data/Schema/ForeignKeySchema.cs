using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Kros.Data.Schema
{
    /// <summary>
    /// The schema of the foreign key of the database table.
    /// </summary>
    public class ForeignKeySchema
    {
        #region Fields

        private readonly List<string> _primaryKeyTableColumns = new List<string>();
        private readonly List<string> _foreignKeyTableColumns = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a definition of foreign key with the <paramref name="name"/>.
        /// Column <paramref name="primaryKeyTableColumn"/> in parent table <paramref name="primaryKeyTableName"/> is
        /// referenced in column <paramref name="foreignKeyTableColumn"/> of child table <paramref name="foreignKeyTableName"/>.
        /// </summary>
        /// <param name="name">Name of the foreign key.</param>
        /// <param name="primaryKeyTableName"><inheritdoc cref="PrimaryKeyTableName" select="summary"/>.</param>
        /// <param name="primaryKeyTableColumn">Column name in primary key table.</param>
        /// <param name="foreignKeyTableName"><inheritdoc cref="ForeignKeyTableName" select="summary"/>.</param>
        /// <param name="foreignKeyTableColumn">Column name in foreign key table.</param>
        /// <exception cref="ArgumentNullException">Value of any parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of any parameter is empty string, or string containing only
        /// whitespace characters.</exception>
        public ForeignKeySchema(
            string name,
            string primaryKeyTableName,
            string primaryKeyTableColumn,
            string foreignKeyTableName,
            string foreignKeyTableColumn)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            PrimaryKeyTableName = Check.NotNullOrWhiteSpace(primaryKeyTableName, nameof(primaryKeyTableName));
            Check.NotNullOrWhiteSpace(primaryKeyTableColumn, nameof(primaryKeyTableColumn));
            ForeignKeyTableName = Check.NotNullOrWhiteSpace(foreignKeyTableName, nameof(foreignKeyTableName));
            Check.NotNullOrWhiteSpace(foreignKeyTableColumn, nameof(foreignKeyTableColumn));

            PrimaryKeyTableColumns = new ReadOnlyCollection<string>(_primaryKeyTableColumns);
            ForeignKeyTableColumns = new ReadOnlyCollection<string>(_foreignKeyTableColumns);
            _primaryKeyTableColumns.Add(primaryKeyTableColumn);
            _foreignKeyTableColumns.Add(foreignKeyTableColumn);
        }

        /// <summary>
        /// Creates a definition of foreign key with the <paramref name="name"/>.
        /// Columns <paramref name="primaryKeyTableColumns"/> in parent table <paramref name="primaryKeyTableName"/> are
        /// referenced in columns <paramref name="foreignKeyTableColumns"/> of child table <paramref name="foreignKeyTableName"/>.
        /// </summary>
        /// <param name="name">Name of the foreign key.</param>
        /// <param name="primaryKeyTableName"><inheritdoc cref="PrimaryKeyTableName" select="summary"/>.</param>
        /// <param name="primaryKeyTableColumns">List of columns in parent table.</param>
        /// <param name="foreignKeyTableName"><inheritdoc cref="ForeignKeyTableName" select="summary"/>.</param>
        /// <param name="foreignKeyTableColumns">List of columns in child table.</param>
        /// <exception cref="ArgumentNullException">Value of any argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="name"/>, <paramref name="primaryKeyTableName"/> or
        /// <paramref name="foreignKeyTableName"/> is empty string, or string containing only whitespace characters.</item>
        /// <item><paramref name="primaryKeyTableColumns"/> or <paramref name="foreignKeyTableColumns"/> is empty
        /// (contains no items).</item>
        /// </list></exception>
        public ForeignKeySchema(
            string name,
            string primaryKeyTableName,
            IEnumerable<string> primaryKeyTableColumns,
            string foreignKeyTableName,
            IEnumerable<string> foreignKeyTableColumns)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            PrimaryKeyTableName = Check.NotNullOrWhiteSpace(primaryKeyTableName, nameof(primaryKeyTableName));
            Check.NotNull(primaryKeyTableColumns, nameof(primaryKeyTableColumns));
            ForeignKeyTableName = Check.NotNullOrWhiteSpace(foreignKeyTableName, nameof(foreignKeyTableName));
            Check.NotNull(foreignKeyTableColumns, nameof(foreignKeyTableColumns));
            Check.GreaterThan(primaryKeyTableColumns.Count(), 0, nameof(primaryKeyTableColumns));
            Check.GreaterThan(foreignKeyTableColumns.Count(), 0, nameof(foreignKeyTableColumns));

            PrimaryKeyTableColumns = new ReadOnlyCollection<string>(_primaryKeyTableColumns);
            ForeignKeyTableColumns = new ReadOnlyCollection<string>(_foreignKeyTableColumns);
            _primaryKeyTableColumns.AddRange(primaryKeyTableColumns);
            _foreignKeyTableColumns.AddRange(foreignKeyTableColumns);
        }

        #endregion

        #region Common

        /// <summary>
        /// Name of the foreign key.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Name of the table, where the primary key is.
        /// </summary>
        public string PrimaryKeyTableName { get; }

        /// <summary>
        /// List of columns in primary key table.
        /// </summary>
        public ReadOnlyCollection<string> PrimaryKeyTableColumns { get; }

        /// <summary>
        /// Name of the child table.
        /// </summary>
        public string ForeignKeyTableName { get; }

        /// <summary>
        /// List of columns in child table.
        /// </summary>
        public ReadOnlyCollection<string> ForeignKeyTableColumns { get; }

        /// <summary>
        /// The rule, what to do when record in parent table is deleted.
        /// </summary>
        public ForeignKeyRule DeleteRule { get; set; } = ForeignKeyRule.NoAction;

        /// <summary>
        /// The rule, what to do when record in parent table is updated.
        /// </summary>
        public ForeignKeyRule UpdateRule { get; set; } = ForeignKeyRule.NoAction;

        /// <summary>
        /// Table to which this foreign key belongs.
        /// </summary>
        public TableSchema Table { get; internal set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(50);

            sb.AppendFormat("Foreign Key {0}: ", Name);
            ToStringAddTable(sb, PrimaryKeyTableName, PrimaryKeyTableColumns);
            sb.Append(", ");
            ToStringAddTable(sb, ForeignKeyTableName, ForeignKeyTableColumns);
            sb.AppendFormat(", On Delete = {0}, On Update = {1}", DeleteRule.ToString(), UpdateRule.ToString());

            return sb.ToString();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void ToStringAddTable(StringBuilder sb, string tableName, IEnumerable<string> columns)
        {
            sb.Append(tableName);
            sb.Append(" (");
            bool first = true;
            foreach (string column in columns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(column);
            }
            sb.Append(")");
        }

        #endregion
    }
}
