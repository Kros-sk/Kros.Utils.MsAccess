using Kros.Utils;
using System;
using System.Text;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Schema of a database table.
    /// </summary>
    public class TableSchema
    {
        #region Fields

        private IndexSchema _primaryKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <c>TableSchema</c> with specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Meno tabuľky. Je povinné.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="name"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public TableSchema(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Creates an instance of <c>TableSchema</c> with specified <paramref name="name"/>,
        /// which belongs to <paramref name="database"/>.
        /// </summary>
        /// <param name="database">Database into which table belongs to. Value can be <see langword="null"/>.</param>
        /// <param name="name">Table's name.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="name"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public TableSchema(DatabaseSchema database, string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));

            Database = database;
            Columns = new ColumnSchemaCollection(this);
            Indexes = new IndexSchemaCollection(this);
            ForeignKeys = new ForeignKeySchemaCollection(this);
        }

        #endregion

        #region Common

        /// <summary>
        /// Table's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Database to which table belongs.
        /// </summary>
        public DatabaseSchema Database { get; internal set; }

        /// <summary>
        /// Columns of the table.
        /// </summary>
        public ColumnSchemaCollection Columns { get; }

        /// <summary>
        /// Table's primary key.
        /// </summary>
        /// <remarks>
        /// If table does not have a primary key, value is <see langword="null"/>.
        /// </remarks>
        public IndexSchema PrimaryKey => _primaryKey;

        /// <summary>
        /// Sets primary key with name <paramref name="primaryKeyName"/> and flag <paramref name="clustered"/>.
        /// </summary>
        /// <param name="primaryKeyName">Primary key name.</param>
        /// <param name="clustered">If <see langword="true"/>, primary key is <c>CLUSTERED</c>.</param>
        /// <returns>Created primary key or <see langword="null"/> value, if primary key name was not specified.</returns>
        /// <remarks>If <paramref name="primaryKeyName"/> is <see langword="null"/> or empty string or white space string,
        /// primary key is removed (value of <see cref="PrimaryKey"/> will be <see langword="null"/>).</remarks>
        public IndexSchema SetPrimaryKey(string primaryKeyName, bool clustered)
        {
            _primaryKey = string.IsNullOrWhiteSpace(primaryKeyName)
                ? null
                : new IndexSchema(primaryKeyName, IndexType.PrimaryKey, clustered);
            return _primaryKey;
        }

        /// <summary>
        /// List of table's indexes.
        /// </summary>
        public IndexSchemaCollection Indexes { get; }

        /// <summary>
        /// List of table's foreign keys.
        /// </summary>
        public ForeignKeySchemaCollection ForeignKeys { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(50);
            sb.Append("Table ");
            sb.Append(Name);
            sb.Append(": Primary Key = ");
            if ((PrimaryKey == null) || (PrimaryKey.Columns.Count == 0))
            {
                sb.Append("*not set*");
            }
            else
            {
                bool first = true;
                foreach (IndexColumnSchema column in PrimaryKey.Columns)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.Append(column.Name);
                }
            }

            return sb.ToString();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
