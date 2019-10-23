using FluentAssertions;
using Kros.Data.BulkActions;
using Kros.Data.BulkActions.MsAccess;
using Kros.Data.MsAccess;
using Kros.UnitTests;
using Kros.Utils.UnitTests;
using Kros.Utils.UnitTests.Data.BulkActions;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data.BulkActions
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

        private class BulkUpdateItemComposite
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public string DataValue { get; set; }
            public override bool Equals(object obj)
            {
                if (obj is BulkUpdateItemComposite item)
                {
                    return (Id1 == item.Id1) && (Id2 == item.Id2) && (DataValue == item.DataValue);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode()
                => Id1.GetHashCode() ^ Id2.GetHashCode() ^ (DataValue is null ? 0 : DataValue.GetHashCode());
        }

        #endregion

        #region Constants

        private const string AccdbFileName = "MsAccessBulkUpdate.accdb";
        private const string MdbFileName = "MsAccessBulkUpdate.mdb";
        private const string TableName = "BulkUpdateTest";
        private const string Composite_TableName = "BulkUpdateTestCompositePK";
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
        public async Task BulkUpdateDataWithCompositePkAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                await BulkUpdateDataWithCompositePk(helper.Connection);
            }
        }

        [SkippableFact]
        public async Task BulkUpdateDataWithCompositePkMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                await BulkUpdateDataWithCompositePk(helper.Connection);
            }
        }

        private async Task BulkUpdateDataWithCompositePk(OleDbConnection cn)
        {
            List<BulkUpdateItemComposite> actualData = null;

            using (var bulkUpdate = new MsAccessBulkUpdate(cn))
            {
                var dataToUpdate = new EnumerableDataReader<BulkUpdateItemComposite>(
                    new[] {
                        new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, DataValue = "lorem ipsum 1" },
                        new BulkUpdateItemComposite() { Id1 = 2, Id2 = 2, DataValue = "lorem ipsum 2" },
                        new BulkUpdateItemComposite() { Id1 = 3, Id2 = 2, DataValue = "lorem ipsum 3" }
                    },
                    new[] { nameof(BulkUpdateItemComposite.Id1), nameof(BulkUpdateItemComposite.Id2), nameof(BulkUpdateItemComposite.DataValue) });

                bulkUpdate.DestinationTableName = Composite_TableName;
                bulkUpdate.PrimaryKeyColumn = nameof(BulkUpdateItemComposite.Id1) + ", " + nameof(BulkUpdateItemComposite.Id2);
                await bulkUpdate.UpdateAsync(dataToUpdate);

                actualData = LoadDataForTableWithCompositePk(cn, Composite_TableName);
            }

            actualData.Should().Equal(new List<BulkUpdateItemComposite>(new[]
            {
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 1, DataValue = "1 - 1" },
                new BulkUpdateItemComposite() { Id1 = 1, Id2 = 2, DataValue = "lorem ipsum 1" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 1, DataValue = "2 - 1" },
                new BulkUpdateItemComposite() { Id1 = 2, Id2 = 2, DataValue = "lorem ipsum 2" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 1, DataValue = "3 - 1" },
                new BulkUpdateItemComposite() { Id1 = 3, Id2 = 2, DataValue = "lorem ipsum 3" },
            }));
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private List<BulkUpdateItemComposite> LoadDataForTableWithCompositePk(OleDbConnection cn, string tableName)
        {
            var data = new List<BulkUpdateItemComposite>();

            using (var cmd = new OleDbCommand($"SELECT [Id1], [Id2], [DataValue] FROM [{tableName}] ORDER BY [Id1], [Id2]", cn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new BulkUpdateItemComposite() { Id1 = reader.GetInt32(0), Id2 = reader.GetInt32(1), DataValue = reader.GetString(2) });
                }
            }
            return data;
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
