using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using System.Collections.Generic;

namespace Kros.Examples
{
    class BulkInsertExamples
    {
        private IEnumerable<Item> GetData()
        {
            return null;
        }

        #region BulkInsert
        private class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public void InsertManyItems()
        {
            IEnumerable<Item> data = GetData();

            using (var reader = new EnumerableDataReader<Item>(data, new string[] { "Id", "Name" }))
            {
                using (var bulkInsert = new SqlServerBulkInsert("connection string"))
                {
                    bulkInsert.Insert(reader);
                }
            }
        }
        #endregion
    }
}
