using Kros.Properties;
using System;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Defines the mapping between a column in data source and a column in the destination table for <see cref="IBulkInsert"/>.
    /// </summary>
    public class BulkInsertColumnMapping
    {
        private const int NoOrdinal = -1;

        private string _sourceName = string.Empty;
        private int _sourceOrdinal = NoOrdinal;
        private string _destinationName = string.Empty;
        private int _destinationOrdinal = NoOrdinal;

        /// <summary>
        /// Default constructor that initializes a new <code>BulkInsertColumnMapping</code> object.
        /// </summary>
        public BulkInsertColumnMapping()
        {
        }

        /// <summary>
        /// Creates a new column mapping, using column ordinals to refer to source and destination columns.
        /// </summary>
        /// <param name="sourceOrdinal">The ordinal position of the source column within the data source.</param>
        /// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
        public BulkInsertColumnMapping(int sourceOrdinal, int destinationOrdinal)
        {
            SourceOrdinal = sourceOrdinal;
            DestinationOrdinal = destinationOrdinal;
        }

        /// <summary>
        /// Creates a new column mapping, using a column ordinal to refer to the source column and
        /// a column name for the target column.
        /// </summary>
        /// <param name="sourceOrdinal">The ordinal position of the source column within the data source.</param>
        /// <param name="destinationName">The name of the destination column within the destination table.</param>
        public BulkInsertColumnMapping(int sourceOrdinal, string destinationName)
        {
            SourceOrdinal = sourceOrdinal;
            DestinationName = destinationName;
        }

        /// <summary>
        /// Creates a new column mapping, using a column name to refer to the source column and
        /// a column ordinal for the target column.
        /// </summary>
        /// <param name="sourceName">The name of the source column within the data source.</param>
        /// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
        public BulkInsertColumnMapping(string sourceName, int destinationOrdinal)
        {
            SourceName = sourceName;
            DestinationOrdinal = destinationOrdinal;
        }

        /// <summary>
        /// Creates a new column mapping, using column names to refer to source and destination columns.
        /// </summary>
        /// <param name="sourceName">The name of the source column within the data source.</param>
        /// <param name="destinationName">The name of the destination column within the destination table.</param>
        public BulkInsertColumnMapping(string sourceName, string destinationName)
        {
            SourceName = sourceName;
            DestinationName = destinationName;
        }

        /// <summary>
        /// Name of the column being mapped in the data source. When set, <see cref="SourceOrdinal"/> is set to <code>-1</code>.
        /// </summary>
        public string SourceName
        {
            get => _sourceName;
            set
            {
                _sourceOrdinal = NoOrdinal;
                _sourceName = value;
            }
        }

        /// <summary>
        /// The ordinal position of the source column within the data source.
        /// When set, <see cref="SourceName"/> is set to empty string.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Value is less than 0.</exception>
        public int SourceOrdinal
        {
            get => _sourceOrdinal;
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException(Resources.InvalidBulkInsertColumnMappingOrdinal);
                }
                _sourceName = string.Empty;
                _sourceOrdinal = value;
            }
        }

        /// <summary>
        /// Name of the column being mapped in the destination database table.
        /// When set, <see cref="DestinationOrdinal"/> is set to <code>-1</code>.
        /// </summary>
        public string DestinationName
        {
            get => _destinationName ?? string.Empty;
            set
            {
                _destinationOrdinal = NoOrdinal;
                _destinationName = value;
            }
        }

        /// <summary>
        /// Ordinal value of the destination column within the destination table.
        /// When set, <see cref="DestinationName"/> is set to empty string.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Value is less than 0.</exception>
        public int DestinationOrdinal
        {
            get => _destinationOrdinal;
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException(Resources.InvalidBulkInsertColumnMappingOrdinal);
                }
                _destinationName = string.Empty;
                _destinationOrdinal = value;
            }
        }

        /// <summary>
        /// Returns mapping in a form "Source -> Destination".
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(SourceOrdinal > NoOrdinal ? SourceOrdinal.ToString() : SourceName);
            sb.Append(" -> ");
            sb.Append(DestinationOrdinal > NoOrdinal ? DestinationOrdinal.ToString() : DestinationName);

            return sb.ToString();
        }
    }
}
