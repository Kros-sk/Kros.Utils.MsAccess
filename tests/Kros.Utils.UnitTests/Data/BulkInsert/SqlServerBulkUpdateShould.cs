using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class SqlServerBulkUpdateShould : DatabaseTestBase
    {
        #region Nested types

        private class BulkUpdateItem
        {
            public int Id { get; set; }
            public int? ColInt32 { get; set; }
            public double? ColDouble { get; set; }
            public DateTime? ColDate { get; set; }
            public Guid? ColGuid { get; set; }
            public bool? ColBool { get; set; }
            public string ColShortText { get; set; }

            public BulkUpdateItem Clone() => (BulkUpdateItem)MemberwiseClone();
        }

        #endregion

        #region Constants

        private const double DOUBLE_MINIMUM = -999999999999999999999.999999999999;
        private const double DOUBLE_MAXIMUM = 999999999999999999999.999999999999;
        private const string DATABASE_NAME = "KrosUtilsTestBulkUpdate";
        private const string TABLE_NAME = "BulkUpdateTest";
        private const string PRIMARY_KEY_COLUMN = "Id";
        private const string SHORT_TEXT_ACTION = "dolor sit amet";

        private string CreateTable_BulkUpdateTest =
            $@"CREATE TABLE[dbo].[{TABLE_NAME}] (
                   [Id] [int] NOT NULL,
                   [ColInt32] [int] NULL,
                   [ColDouble] [float] NULL,
                   [ColDate] [datetime2] (7) NULL,
                   [ColGuid] [uniqueidentifier] NULL,
                   [ColBool] [bit] NULL,
                   [ColShortText] [nvarchar] (20) NULL
               
                   CONSTRAINT [PK_{TABLE_NAME}] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
               
               ) ON [PRIMARY];
               ";
        private string Insert_BulkUpdateTest =
            $@"INSERT INTO {TABLE_NAME} 
                      VALUES (1, 0, 0.0, '1900-01-01 00:00:00', '00000000-0000-0000-0000-000000000000', 0, '')
               INSERT INTO {TABLE_NAME} (Id)
                      VALUES (2)
               INSERT INTO {TABLE_NAME} 
                      VALUES (3, 1, 9.9, '2018-01-30 10:26:36', 'abcdefab-1234-5678-9123-abcdefabcdef', 1, 'ipsum')";

        #endregion

        #region DatabaseTestBase Overrides

        protected override string BaseDatabaseName => DATABASE_NAME;

        protected override IEnumerable<string> DatabaseInitScripts =>
            new string[] { CreateTable_BulkUpdateTest, Insert_BulkUpdateTest };

        #endregion

        #region Tests

        [Fact]
        public void BulkUpdateDataFromIDataReaderSynchronouslyWithoutDeadLock()
        {
            AsyncContext.Run(() =>
            {
                DataTable expectedData = CreateExpectedData();
                DataTable actualData = null;

                using (IDataReader reader = expectedData.CreateDataReader())
                using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
                {
                    bulkUpdate.DestinationTableName = TABLE_NAME;
                    bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                    bulkUpdate.Update(reader);
                }

                actualData = LoadData(ServerHelper.Connection);

                SqlServerBulkHelper.CompareTables(actualData, expectedData);
            });
        }

        [Fact]
        public async Task BulkUpdateDataFromIDataReaderAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();
            DataTable actualData = null;

            using (IDataReader reader = expectedData.CreateDataReader())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                await bulkUpdate.UpdateAsync(reader);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromIBulkInsertDataReader()
        {
            DataTable expectedData = CreateExpectedData();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                bulkUpdate.Update(reader);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkUpdateDataFromIBulkInsertDataReaderAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                await bulkUpdate.UpdateAsync(reader);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromIDataReaderWithAction()
        {
            DataTable expectedData = CreateExpectedDataWithAction();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                bulkUpdate.TempTableAction = UpdateTempItems;
                bulkUpdate.Update(reader);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkUpdateDataFromDataTable()
        {
            DataTable expectedData = CreateExpectedData();
            DataTable actualData = null;

            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                bulkUpdate.Update(expectedData);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkUpdateDataFromDataTableAsynchronously()
        {
            DataTable expectedData = CreateExpectedData();
            DataTable actualData = null;

            using (var bulkUpdate = new SqlServerBulkUpdate(ServerHelper.Connection))
            {
                bulkUpdate.DestinationTableName = TABLE_NAME;
                bulkUpdate.PrimaryKeyColumn = PRIMARY_KEY_COLUMN;
                await bulkUpdate.UpdateAsync(expectedData);
            }

            actualData = LoadData(ServerHelper.Connection);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        #endregion

        #region Helpers

        private static List<BulkUpdateItem> _rawData = new List<BulkUpdateItem>
        {
            new BulkUpdateItem()
            {
                Id = 1,
                ColInt32 = 123,
                ColDouble = 123456.654321,
                ColDate =new DateTime(1978, 12, 10, 7, 30, 59),
                ColGuid =new Guid("abcdef00-1234-5678-9000-abcdefabcdef"),
                ColBool = true,
                ColShortText = "lorem ipsum",
            },
            new BulkUpdateItem()
            {
                Id = 2,
                ColInt32 = int.MinValue,
                ColDouble = DOUBLE_MINIMUM,
                ColDate =new DateTime(1900, 1, 1),
                ColGuid =new Guid("abcdef00-1234-5678-9000-abcdefabcdef"),
                ColBool = false,
                ColShortText = "dolor sit amet",
            },
            new BulkUpdateItem()
            {
                Id = 3,
                ColInt32 = int.MaxValue,
                ColDouble = DOUBLE_MAXIMUM,
                ColDate = DateTime.MaxValue,
                ColGuid = Guid.Empty,
                ColBool = true,
                ColShortText = string.Empty,
            }
        };

        private void UpdateTempItems(IDbConnection connection, IDbTransaction transaction, string tempTableName)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = $"UPDATE [{tempTableName}] " +
                                  $"SET ColGuid = '{Guid.Empty}', ColShortText = '{SHORT_TEXT_ACTION}'";
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable LoadData(SqlConnection cn)
        {
            DataTable data = new DataTable(TABLE_NAME);

            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {TABLE_NAME}", cn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.FillSchema(data, SchemaType.Source);
                adapter.Fill(data);
            }

            return data;
        }

        private DataTable CreateExpectedData()
        {
            DataTable table = CreateBulkUpdateDataTable();
            FillBulkUpdateDataTable(table, _rawData);

            return table;
        }

        private DataTable CreateExpectedDataWithAction()
        {
            var table = CreateBulkUpdateDataTable();

            FillBulkUpdateDataTable(table, GetActionRawData());

            return table;
        }

        private List<BulkUpdateItem> GetActionRawData()
        {
            var actionRawData = new List<BulkUpdateItem>();
            BulkUpdateItem cloneItem = null;

            foreach (var item in _rawData)
            {
                cloneItem = item.Clone();
                cloneItem.ColGuid = Guid.Empty;
                cloneItem.ColShortText = SHORT_TEXT_ACTION;
                actionRawData.Add(cloneItem);
            }

            return actionRawData;
        }

        private DataTable CreateBulkUpdateDataTable()
        {
            DataTable table = new DataTable("data");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ColInt32", typeof(int));
            table.Columns.Add("ColDouble", typeof(double));
            table.Columns.Add("ColDate", typeof(DateTime));
            table.Columns.Add("ColGuid", typeof(Guid));
            table.Columns.Add("ColBool", typeof(bool));
            table.Columns.Add("ColShortText", typeof(string));

            table.PrimaryKey = new DataColumn[] { table.Columns[PRIMARY_KEY_COLUMN] };

            return table;
        }

        private void FillBulkUpdateDataTable(DataTable table, List<BulkUpdateItem> rawData)
        {
            foreach (var rawItem in rawData)
            {
                AddBulkUpdateDataRow(table, rawItem);
            }
        }

        private void AddBulkUpdateDataRow(DataTable table, BulkUpdateItem item)
        {
            DataRow row = table.NewRow();

            foreach (var itemProperty in typeof(BulkUpdateItem).GetProperties())
            {
                row[itemProperty.Name] = itemProperty.GetValue(item);
            }

            table.Rows.Add(row);
        }

        private IBulkActionDataReader CreateDataReaderForUpdate()
        {
            var columnNames = typeof(BulkUpdateItem).GetProperties().Select(p => p.Name);

            return new EnumerableDataReader<BulkUpdateItem>(_rawData, columnNames);
        }

        #endregion
    }
}
