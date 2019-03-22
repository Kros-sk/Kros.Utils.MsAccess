using System;
using System.Data;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Interface for fast data updating in database.
    /// </summary>
    public interface IBulkUpdate : IDisposable
    {
        /// <summary>
        /// Destination table name in database.
        /// </summary>
        string DestinationTableName { get; set; }

        /// <summary>
        /// Action, executed on temporary table.
        /// </summary>
        /// <remarks>
        /// Action, which will be executed on temp table (additional modification of data).
        /// <list type="bullet">
        /// <item>
        /// <c>IDbConnection</c> - connection on temporary table,
        /// </item>
        /// <item>
        /// <c>IDbTransaction</c> - transaction on temporary table,
        /// </item>
        /// <item>
        /// <c>string</c> - temporary table name.
        /// </item>
        /// </list>
        /// </remarks>
        Action<IDbConnection, IDbTransaction, string> TempTableAction { get; set; }

        /// <summary>
        /// Primary key.
        /// </summary>
        string PrimaryKeyColumn { get; set; }

        /// <summary>
        /// Updates all data from source <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        void Update(IBulkActionDataReader reader);

        /// <summary>
        /// Asynchronously updates all data from source <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task UpdateAsync(IBulkActionDataReader reader);

        /// <summary>
        /// Updates all data from source <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        void Update(IDataReader reader);

        /// <summary>
        /// Asynchronously updates all data from source <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Data source.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task UpdateAsync(IDataReader reader);

        /// <summary>
        /// Updates all data from table <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Data source.</param>
        void Update(DataTable table);

        /// <summary>
        /// Asynchronously updates all data from table <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Data source.</param>
        /// <returns>
        /// A task that represents the asynchronous Insert operation.
        /// </returns>
        Task UpdateAsync(DataTable table);
    }
}
