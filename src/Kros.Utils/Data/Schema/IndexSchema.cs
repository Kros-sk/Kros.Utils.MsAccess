using Kros.Properties;
using Kros.Utils;
using System;
using System.Text;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Schema of a table's index.
    /// </summary>
    public class IndexSchema
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of index with name <paramref name="indexName"/>.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema(string indexName)
            : this(indexName, IndexType.Index, false)
        {
        }

        /// <summary>
        /// Creates an instance of index of type <paramref name="indexType"/>, with name <paramref name="indexName"/>.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="indexType">Type of the index.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema(string indexName, IndexType indexType)
            : this(indexName, indexType, false)
        {
        }

        /// <summary>
        /// Creates an instance of index of type <paramref name="indexType"/>, with name <paramref name="indexName"/> and
        /// setting if the index is <paramref name="clustered"/>.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="indexType">Type of the index.</param>
        /// <param name="clustered">Specifies, if the index is <c>CLUSTERED</c>.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="indexName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="indexName"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public IndexSchema(string indexName, IndexType indexType, bool clustered)
        {
            Check.NotNullOrWhiteSpace(indexName, nameof(indexName));

            Name = indexName;
            IndexType = indexType;
            Clustered = clustered;
            Columns = new IndexColumnSchemaCollection(this);
        }

        #endregion

        #region Common

        private string _name;

        /// <summary>
        /// Name of the index. It is not possible to set the name, if the index already belongs to some table
        /// (value of <see cref="Table"/> is not <see langword="null"/>)
        /// </summary>
        /// <exception cref="InvalidOperationException">Attempt to change the name, but index already belongs to some table.
        /// </exception>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (Table != null)
                {
                    throw new InvalidOperationException(string.Format(
                        Resources.CannotChangeIndexNameWhenIndexBelongsToTable, _name, Table.Name));
                }
                _name = Check.NotNullOrWhiteSpace(value, nameof(value));
            }
        }

        /// <summary>
        /// Index type.
        /// </summary>
        public IndexType IndexType { get; }

        /// <summary>
        /// Specifies, if the index is <c>CLUSTERED</c>.
        /// </summary>
        bool Clustered { get; set; }

        /// <summary>
        /// List of index's columns.
        /// </summary>
        public IndexColumnSchemaCollection Columns { get; }

        /// <summary>
        /// Table, to which index belongs.
        /// </summary>
        public TableSchema Table { get; internal set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(50);

            sb.AppendFormat("Index {0}", Name);
            if ((IndexType != IndexType.Index) || Clustered)
            {
                sb.Append(" (");
                switch (IndexType)
                {
                    case IndexType.PrimaryKey:
                        sb.Append("primary key");
                        break;

                    case IndexType.UniqueKey:
                        sb.Append("unique");
                        break;
                }

                if (Clustered)
                {
                    if (IndexType != IndexType.Index)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("clustered");
                }
                sb.Append(")");
            }
            sb.Append(": ");

            bool first = true;
            foreach (IndexColumnSchema column in Columns)
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
                if (column.Order == SortOrder.Descending)
                {
                    sb.Append(" DESC");
                }
            }

            return sb.ToString();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
