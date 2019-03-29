using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using System.Collections.Generic;

namespace Kros.Utils.Examples
{
    class BulkUpdateExamples
    {
        #region BulkUpdate
        private class BulkUpdateItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public void UpdateManyItems()
        {
            IEnumerable<BulkUpdateItem> data = GetItems();

            using (var reader = new EnumerableDataReader<BulkUpdateItem>(data, new string[] { "Id", "Name" }))
            {
                using (var bulkUpdate = new SqlServerBulkUpdate("connection string"))
                {
                    bulkUpdate.DestinationTableName = "TableName";
                    bulkUpdate.PrimaryKeyColumn = "Id";
                    bulkUpdate.Update(reader);
                }
            }
        }
        #endregion

        private IEnumerable<BulkUpdateItem> GetItems()
        {
            return new List<BulkUpdateItem>() { new BulkUpdateItem { Id = 1, Name = "Test" } };
        }
    }
}
