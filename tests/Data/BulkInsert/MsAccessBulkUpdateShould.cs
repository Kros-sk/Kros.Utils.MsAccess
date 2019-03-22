using Kros.Data.BulkActions;
using Kros.Data.BulkActions.MsAccess;
using Kros.Data.MsAccess;
using Kros.UnitTests;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class MsAccessBulkUpdateShould
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

        private const string AccdbFileName = "MsAccessBulkUpdate.accdb";
        private const string MdbFileName = "MsAccessBulkUpdate.mdb";
        private const string TableName = "BulkUpdateTest";
        private const string PrimaryKeyColumn = "Id";
        private const string ShortTextAction = "dolor sit amet";
        private const double DoubleMinimum = -999999999999999999999.999999999999;
        private const double DoubleMaximum = 999999999999999999999.999999999999;

        #endregion

        #region Tests

        [SkippableFact]
        public void BulkUpdateDataFromDataTableIntoAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                DataTableBulkUpdateCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void BulkUpdateDataFromDataTableIntoMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                DataTableBulkUpdateCore(helper.Connection);
            }
        }

        private void DataTableBulkUpdateCore(OleDbConnection cn)
        {
            DataTable expectedData = CreateExpectedData();

            using (var bulkUpdate = new MsAccessBulkUpdate(cn))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.Update(expectedData);
            }

            DataTable actualData = LoadData(cn);
            MsAccessBulkHelper.CompareTables(actualData, expectedData);
        }

        [SkippableFact]
        public void BulkUpdateDataFromIBulkActionDataReaderIntoAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                IBulkActionDataReaderBulkUpdateCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void BulkUpdateDataFromIBulkActionDataReaderIntoMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                IBulkActionDataReaderBulkUpdateCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void BulkUpdateDataWithActionFromIBulkActionDataReaderAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                IBulkActionDataReaderBulkUpdateCore(helper.Connection, CreateExpectedDataWithAction(), UpdateTempItems);
            }
        }

        [SkippableFact]
        public void BulkUpdateDataWithActionFromIBulkActionDataReaderIntoMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                IBulkActionDataReaderBulkUpdateCore(helper.Connection, CreateExpectedDataWithAction(), UpdateTempItems);
            }
        }

        [SkippableFact]
        public void BulkUpdateDataFromIDataReaderIntoMdbSynchronouslyWithoutDeadLock()
        {
            AsyncContext.Run(() =>
            {
                Helpers.SkipTestIfJetProviderNotAvailable();
                using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
                {
                    DataTable expectedData = CreateExpectedData();

                    using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
                    using (var bulkUpdate = new MsAccessBulkUpdate(helper.Connection))
                    {
                        bulkUpdate.DestinationTableName = TableName;
                        bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                        bulkUpdate.Update(reader);
                    }

                    DataTable actualData = LoadData(helper.Connection);
                    MsAccessBulkHelper.CompareTables(actualData, expectedData);
                }
            });
        }

        [SkippableFact]
        public async Task BulkUpdateDataFromIDataReaderIntoMdbAsynchronously()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                DataTable expectedData = CreateExpectedData();

                using (IDataReader reader = expectedData.CreateDataReader())
                using (var bulkUpdate = new MsAccessBulkUpdate(helper.Connection))
                {
                    bulkUpdate.DestinationTableName = TableName;
                    bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                    await bulkUpdate.UpdateAsync(reader);
                }

                DataTable actualData = LoadData(helper.Connection);
                MsAccessBulkHelper.CompareTables(actualData, expectedData);
            }
        }

        private void IBulkActionDataReaderBulkUpdateCore(OleDbConnection cn)
        {
            IBulkActionDataReaderBulkUpdateCore(cn, CreateExpectedData(), null);
        }

        private void IBulkActionDataReaderBulkUpdateCore(
            OleDbConnection cn,
            DataTable expectedData,
            Action<IDbConnection, IDbTransaction, string> action)
        {
            using (IBulkActionDataReader reader = CreateDataReaderForUpdate())
            using (var bulkUpdate = new MsAccessBulkUpdate(cn))
            {
                bulkUpdate.DestinationTableName = TableName;
                bulkUpdate.PrimaryKeyColumn = PrimaryKeyColumn;
                bulkUpdate.TempTableAction = action;
                bulkUpdate.Update(reader);
            }

            DataTable actualData = LoadData(cn);
            MsAccessBulkHelper.CompareTables(actualData, expectedData);
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
                ColDouble = DoubleMinimum,
                ColDate =new DateTime(1900, 1, 1),
                ColGuid =new Guid("abcdef00-1234-5678-9000-abcdefabcdef"),
                ColBool = false,
                ColShortText = "dolor sit amet",
            },
            new BulkUpdateItem()
            {
                Id = 3,
                ColInt32 = int.MaxValue,
                ColDouble = DoubleMaximum,
                ColDate = DateTime.MaxValue,
                ColGuid = Guid.Empty,
                ColBool = true,
                ColShortText = string.Empty,
            }
        };

        private MsAccessTestHelper CreateHelper(ProviderType provider, string fileName)
        {
            string resourceName = Helpers.RootNamespaceResources + "." + fileName;
            Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            var ret = new MsAccessTestHelper(provider, sourceStream);
            ret.Connection.Open();

            return ret;
        }

        private void UpdateTempItems(IDbConnection connection, IDbTransaction transaction, string tempTableName)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = $"UPDATE [{tempTableName}] " +
                                  $"SET ColGuid = '{Guid.Empty}', ColShortText = '{ShortTextAction}'";
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable LoadData(OleDbConnection cn)
        {
            DataTable data = new DataTable(TableName);

            using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM {TableName}", cn))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
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
                cloneItem.ColShortText = ShortTextAction;
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

            table.PrimaryKey = new DataColumn[] { table.Columns[PrimaryKeyColumn] };

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
