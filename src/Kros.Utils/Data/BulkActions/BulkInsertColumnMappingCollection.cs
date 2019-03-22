using System.Collections.ObjectModel;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Collection of <see cref="BulkInsertColumnMapping"/> objects that inherits from <see cref="Collection{T}"/>.
    /// </summary>
    public class BulkInsertColumnMappingCollection
        : Collection<BulkInsertColumnMapping>
    {
        /// <summary>
        /// Creates a new <see cref="BulkInsertColumnMapping"/> and adds it to the collection, using ordinals to specify
        /// both source and destination columns.
        /// </summary>
        /// <param name="sourceOrdinal">The ordinal position of the source column within the data source.</param>
        /// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
        public void Add(int sourceOrdinal, int destinationOrdinal)
        {
            Add(new BulkInsertColumnMapping(sourceOrdinal, destinationOrdinal));
        }

        /// <summary>
        /// Creates a new <see cref="BulkInsertColumnMapping"/> and adds it to the collection, using an ordinal for
        /// the source column and a string for the destination column.
        /// </summary>
        /// <param name="sourceOrdinal">The ordinal position of the source column within the data source.</param>
        /// <param name="destinationName">The name of the destination column within the destination table.</param>
        public void Add(int sourceOrdinal, string destinationName)
        {
            Add(new BulkInsertColumnMapping(sourceOrdinal, destinationName));
        }

        /// <summary>
        /// Creates a new <see cref="BulkInsertColumnMapping"/> and adds it to the collection, using a column name
        /// to describe the source column and an ordinal to specify the destination column.
        /// </summary>
        /// <param name="sourceName">The name of the source column within the data source.</param>
        /// <param name="destinationOrdinal">The ordinal position of the destination column within the destination table.</param>
        public void Add(string sourceName, int destinationOrdinal)
        {
            Add(new BulkInsertColumnMapping(sourceName, destinationOrdinal));
        }

        /// <summary>
        /// Creates a new <see cref="BulkInsertColumnMapping"/> and adds it to the collection, using column names
        /// to specify both source and destination columns.
        /// </summary>
        /// <param name="sourceName">The name of the source column within the data source.</param>
        /// <param name="destinationName">The name of the destination column within the destination table.</param>
        public void Add(string sourceName, string destinationName)
        {
            Add(new BulkInsertColumnMapping(sourceName, destinationName));
        }
    }
}
