using Kros.Data.BulkActions;
using Kros.Data.BulkActions.MsAccess;
using Kros.Data.MsAccess;
using Kros.Data.Schema;
using Kros.UnitTests;
using System.Collections.Generic;
using System.Data.OleDb;

namespace Kros.Utils.MsAccess.Examples
{
    class MsAccessExamples
    {
        #region Private helpers

        private class FactAttribute
            : System.Attribute
        {
        }

        private IEnumerable<Item> GetData()
        {
            return null;
        }

        private IEnumerable<BulkUpdateItem> GetItems()
        {
            return new List<BulkUpdateItem>() { new BulkUpdateItem { Id = 1, Name = "Test" } };
        }

        #endregion

        public void LoadSchemaExample()
        {
            #region SchemaLoader
            OleDbConnection cn = new OleDbConnection("MS Access Connection String");

            DatabaseSchema schema = DatabaseSchemaLoader.Default.LoadSchema(cn);
            #endregion
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
                using (var bulkInsert = new MsAccessBulkInsert("connection string"))
                {
                    bulkInsert.Insert(reader);
                }
            }
        }
        #endregion

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
                using (var bulkUpdate = new MsAccessBulkUpdate("connection string"))
                {
                    bulkUpdate.Update(reader);
                }
            }
        }
        #endregion

        #region TestHelper
        private const string BaseDatabasePath = "C:\testfiles\testdatabase.accdb";

        private const string CreateTestTableScript =
        @"CREATE TABLE [TestTable] (
            [Id] number NOT NULL,
            [Name] text(255) NULL,

            CONSTRAINT [PK_TestTable] PRIMARY KEY ([Id])
        )";

        [Fact]
        public void DoSomeTestWithDatabase()
        {
            using (var helper = new MsAccessTestHelper(ProviderType.Ace, BaseDatabasePath, CreateTestTableScript))
            {
                // Do tests with connection helper.Connection.
            }
        }
        #endregion
    }
}
