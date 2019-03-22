using System;
using System.Data;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Interface for fast data inserting into database.
    /// </summary>
    public interface IBulkInsert : IDisposable
    {
        /// <summary>
        /// Row count for batch sent to database. If 0, batch size is not limited.
        /// </summary>
        int BatchSize { get; set; }

        /// <summary>
        /// Timeout for operation in seconds. If 0, duration of operation is not limited.
        /// </summary>
        int BulkInsertTimeout { get; set; }

        /// <summary>
        /// Destination table name in database.
        /// </summary>
        string DestinationTableName { get; set; }

        /// <summary>
        /// Returns a collection of <see cref="BulkInsertColumnMapping"/> items. Column mappings define the relationships
        /// between columns in the data source and columns in the destination. Mappings also specifies which columns
        /// are inserted into database. When mapping is set, only columns in mapping collection are inserted. If no mapping
        /// is set, all columns from data source are inserted.
        /// </summary>
        BulkInsertColumnMappingCollection ColumnMappings { get; }

        /// <summary>
        /// Inserts all data from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        void Insert(IBulkActionDataReader reader);

        /// <summary>
        /// Asynchronously inserts all data from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task InsertAsync(IBulkActionDataReader reader);

        /// <summary>
        /// Inserts all data from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        void Insert(IDataReader reader);

        /// <summary>
        /// Asynchronously inserts all data from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task InsertAsync(IDataReader reader);

        /// <summary>
        /// Inserts all rows from table <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Source table.</param>
        void Insert(DataTable table);

        /// <summary>
        /// Asynchronously inserts all rows from table <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Source table.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task InsertAsync(DataTable table);
    }
}
