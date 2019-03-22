using System.Data.Common;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Creates instances of <see cref="IBulkInsert"/> and <see cref="IBulkUpdate"/> for bulk actions.
    /// </summary>
    public interface IBulkActionFactory
    {
        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <returns>The bulk insert.</returns>
        IBulkInsert GetBulkInsert();

        /// <summary>
        /// Gets the bulk insert.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk insert.</returns>
        IBulkInsert GetBulkInsert(DbTransaction externalTransaction);

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <returns>The bulk update.</returns>
        IBulkUpdate GetBulkUpdate();

        /// <summary>
        /// Gets the bulk update.
        /// </summary>
        /// <param name="externalTransaction">The external transaction.</param>
        /// <returns>The bulk update.</returns>
        IBulkUpdate GetBulkUpdate(DbTransaction externalTransaction);
    }
}
