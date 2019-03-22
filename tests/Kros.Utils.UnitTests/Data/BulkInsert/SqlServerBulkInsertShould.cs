using FluentAssertions;
using Kros.Data.BulkActions;
using Kros.Data.BulkActions.SqlServer;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class SqlServerBulkInsertShould : DatabaseTestBase
    {
        #region Nested types

        public class DataItem
        {
            public int Id { get; set; }
            public string ColNote { get; set; }
            public byte? ColByte { get; set; }
            public int? ColInt32 { get; set; }
            public long? ColInt64 { get; set; }
            public float? ColSingle { get; set; }
            public double? ColDouble { get; set; }
            public decimal? ColDecimal { get; set; }
            public decimal? ColCurrency { get; set; }
            public DateTime? ColDate { get; set; }
            public Guid? ColGuid { get; set; }
            public bool? ColBool { get; set; }
            public string ColShortText { get; set; }
            public string ColLongText { get; set; }
            public string ColNVarcharMax { get; set; }
        }

        public class NonExistingColumnDataItem
        {
            public int Id { get; set; }
            public string ColNote { get; set; }
            public int NonExistingColumn { get; set; }
        }

        #endregion

        #region Constants

        private const float FloatMinimum = (float)-999999999999999999999.999999999999;
        private const float FloatMaximum = -(float)999999999999999999999.999999999999;
        private const double DoubleMinimum = FloatMinimum;
        private const double DoubleMaximum = FloatMaximum;
        private const decimal DecimalMinimum = (decimal)-999999999.99999;
        private const decimal DecimalMaximum = (decimal)999999999.99999;
        private const decimal MoneyMinimum = (decimal)-99999999999999.9999999999;
        private const decimal MoneyMaximum = (decimal)99999999999999.9999999999;

        private const string DATABASE_NAME = "KrosUtilsTestBulkInsert";
        private const string TableName = "BulkInsertTest";
        private readonly string CreateTable_BulkInsertTest =
$@"CREATE TABLE[dbo].[{TableName}] (
    [Id] [int] NOT NULL,
    [ColNote] [nvarchar](255) NULL,
    [ColByte] [tinyint] NULL,
    [ColInt32] [int] NULL,
    [ColInt64] [bigint] NULL,
    [ColSingle] [real] NULL,
    [ColDouble] [float] NULL,
    [ColDecimal] [decimal](18, 5) NULL,
    [ColCurrency] [money] NULL,
    [ColDate] [datetime2](7) NULL,
    [ColGuid] [uniqueidentifier] NULL,
    [ColBool] [bit] NULL,
    [ColShortText] [nvarchar](20) NULL,
    [ColLongText] [ntext] NULL,
    [ColNVarcharMax] [nvarchar](max) NULL,

    CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];
";

        private const string TableName_IgnoreCaseInColumnNames = "BulkInsertTest_IgnoreCaseInColumnNames";
        private readonly string CreateTable_BulkInsertTest_IgnoreCaseInColumnNames =
            $@"CREATE TABLE[dbo].[{TableName_IgnoreCaseInColumnNames}] (
    [ID] [int] NOT NULL,
    [colnote] [nvarchar] (255) NULL,

    CONSTRAINT [PK_TestTable_IgnoreCaseInColumnNames] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];
";

        private const string TableName_PrimitiveDataTypes = "BulkInsertTest_ShortText";
        private readonly string CreateTable_BulkInsertTest_PrimitiveDataTypes =
            $@"CREATE TABLE[dbo].[{TableName_PrimitiveDataTypes}] (
    [ID] [int] NOT NULL IDENTITY(1, 1),
    [ColValue] [nvarchar] (30) NULL,

    CONSTRAINT [PK_TestTable_PrimitiveDataTypes] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];
";

        #endregion

        #region DatabaseTestBase Overrides

        protected override string BaseDatabaseName => DATABASE_NAME;

        protected override IEnumerable<string> DatabaseInitScripts
            => new string[] {
                CreateTable_BulkInsertTest,
                CreateTable_BulkInsertTest_IgnoreCaseInColumnNames,
                CreateTable_BulkInsertTest_PrimitiveDataTypes };

        #endregion

        #region Tests

        [Fact]
        public void BulkInsertDataFromDataTable() => BulkInsertDataFromDataTableCore();

        [Fact]
        public void BulkInsertDataFromDataTableSynchronouslyWithoutDeadLock() =>
            AsyncContext.Run(() => BulkInsertDataFromDataTableCore());

        private void BulkInsertDataFromDataTableCore()
        {
            DataTable expectedData = CreateDataTableDataSource();
            DataTable actualData = null;

            using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
            {
                bulkInsert.DestinationTableName = TableName;
                bulkInsert.Insert(expectedData);
            }
            actualData = LoadData(ServerHelper.Connection, TableName);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public async Task BulkInsertDataFromDataTableAsynchronously()
        {
            DataTable expectedData = CreateDataTableDataSource();
            DataTable actualData = null;

            using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
            {
                bulkInsert.DestinationTableName = TableName;
                await bulkInsert.InsertAsync(expectedData);
            }
            actualData = LoadData(ServerHelper.Connection, TableName);

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkInsertDataFromIDataReaderSynchronouslyWithoutDeadLock()
        {
            AsyncContext.Run(() =>
            {
                DataTable expectedData = CreateDataTableDataSource();
                DataTable actualData = null;

                using (IDataReader reader = expectedData.CreateDataReader())
                {
                    using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                    {
                        bulkInsert.DestinationTableName = TableName;
                        bulkInsert.Insert(reader);
                    }
                    actualData = LoadData(ServerHelper.Connection, TableName);
                }

                SqlServerBulkHelper.CompareTables(actualData, expectedData);
            });
        }

        [Fact]
        public async Task BulkInsertDataFromIDataReaderAsynchronously()
        {
            DataTable expectedData = CreateDataTableDataSource();
            DataTable actualData = null;

            using (IDataReader reader = expectedData.CreateDataReader())
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName;
                    await bulkInsert.InsertAsync(reader);
                }
                actualData = LoadData(ServerHelper.Connection, TableName);
            }

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkInsertDataFromIBulkActionDataReader() => BulkInsertDataFromIBulkActionDataReaderCore();

        [Fact]
        public void BulkInsertDataFromIBulkActionDataReaderSynchronouslyWithoutDeadLock()
            => AsyncContext.Run(() => BulkInsertDataFromIBulkActionDataReaderCore());

        [Fact]
        public async Task BulkInsertDataFromIBulkActionDataReaderAsynchronously()
        {
            DataTable expectedData = CreateDataTableDataSource();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateIDataReaderDataSource(true))
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName;
                    await bulkInsert.InsertAsync(reader);
                }
                actualData = LoadData(ServerHelper.Connection, TableName);
            }

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        private void BulkInsertDataFromIBulkActionDataReaderCore()
        {
            DataTable expectedData = CreateDataTableDataSource();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateIDataReaderDataSource(true))
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName;
                    bulkInsert.Insert(reader);
                }
                actualData = LoadData(ServerHelper.Connection, TableName);
            }

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkInsertDataFromIBulkActionDataReaderIgnoreCaseInColumnNames()
        {
            DataTable expectedData = CreateDataTableDataSource(false);
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateIDataReaderDataSource(false))
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName_IgnoreCaseInColumnNames;
                    bulkInsert.Insert(reader);
                }
                actualData = LoadData(ServerHelper.Connection, TableName_IgnoreCaseInColumnNames);
            }

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void BulkInsertDataFromIDataReaderShortText()
        {
            DataTable expectedData = CreateDataTableShortText();
            DataTable actualData = null;

            using (IBulkActionDataReader reader = CreateIDataReaderShortText())
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName_PrimitiveDataTypes;
                    bulkInsert.Insert(reader);
                }
                actualData = LoadData(ServerHelper.Connection, TableName_PrimitiveDataTypes);
            }

            SqlServerBulkHelper.CompareTables(actualData, expectedData);
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenColumnDoesNotExistInDestinationTable()
        {
            using (IBulkActionDataReader reader = CreateDataSourceWithNonExistingColumn())
            {
                using (SqlServerBulkInsert bulkInsert = new SqlServerBulkInsert(ServerHelper.Connection))
                {
                    bulkInsert.DestinationTableName = TableName_IgnoreCaseInColumnNames;
                    Action action = () => bulkInsert.Insert(reader);
                    action.Should().Throw<InvalidOperationException>()
                        .WithMessage($"*{TableName_IgnoreCaseInColumnNames}*{nameof(NonExistingColumnDataItem.NonExistingColumn)}*");
                }
            }
        }

        #endregion

        #region Helpers

        private static readonly Dictionary<string, Dictionary<string, object>> _rawData = new Dictionary<string, Dictionary<string, object>>
        {
            {
                "ColByte",
                new Dictionary<string, object>
                {
                    { "Byte - 123", (byte) 123 },
                    { "Byte - MinValue", byte.MinValue },
                    { "Byte - MaxValue", byte.MaxValue },
                }
            },
            {
                "ColInt32",
                new Dictionary<string, object>
                {
                    { "Int - 123", 123 },
                    { "Int - MinValue", int.MinValue },
                    { "Int - MaxValue", int.MaxValue },
                }
            },
            {
                "ColInt64",
                new Dictionary<string, object>
                {
                    { "Short - 123", 123L },
                    { "Short - MinValue", long.MinValue },
                    { "Short - MaxValue", long.MaxValue },
                }
            },
            {
                "ColSingle",
                new Dictionary<string, object>
                {
                    { "Single - 123.456", (float) 123456.654321 },
                    { "Single - MinValue", FloatMinimum },
                    { "Single - MaxValue", FloatMaximum },
                }
            },
            {
                "ColDouble",
                new Dictionary<string, object>
                {
                    { "Double - 123.456", 123456.654321 },
                    { "Double - MinValue", DoubleMinimum },
                    { "Double - MaxValue", DoubleMaximum },
                }
            },
            {
                "ColDecimal",
                new Dictionary<string, object>
                {
                    { "Decimal - 123.456", (decimal) 123456.6543 },
                    { "Decimal - MinValue", DecimalMinimum },
                    { "Decimal - MaxValue", DecimalMaximum },
                }
            },
            {
                "ColCurrency",
                new Dictionary<string, object>
                {
                    { "Currency - 123.456", (decimal) 123456.6543 },
                    { "Currency - MinValue", MoneyMinimum },
                    { "Currency - MaxValue", MoneyMaximum },
                }
            },
            {
                "ColDate",
                new Dictionary<string, object>
                {
                    { "DateTime - 10.12.1978 7:30:59", new DateTime(1978, 12, 10, 7, 30, 59) },
                    { "DateTime - 1.1.1900", new DateTime(1900, 1, 1) },
                    { "DateTime - MaxValue", DateTime.MaxValue },
                }
            },
            {
                "ColGuid",
                new Dictionary<string, object>
                {
                    { "Guid", new Guid("abcdef00-1234-5678-9000-abcdefabcdef") },
                }
            },
            {
                "ColBool",
                new Dictionary<string, object>
                {
                    { "Bool - True", true },
                    { "Bool - False", false },
                }
            },
            {
                "ColShortText",
                new Dictionary<string, object>
                {
                    { "ShortText", "lorem ipsum" },
                }
            },
            {
                "ColLongText",
                new Dictionary<string, object>
                {
                    {
                        "LongText",
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer ut ullamcorper nisl. Nulla iaculis " +
                        "scelerisque dui ut molestie. Suspendisse potenti. In hac habitasse platea dictumst. Maecenas " +
                        "pellentesque ante tortor, vitae pellentesque dolor eleifend sed. Cras a commodo arcu. Nulla " +
                        "convallis vulputate quam, vel lobortis mauris feugiat nec. Nullam tincidunt, sapien eu cursus varius, " +
                        "metus lacus ultrices leo, eu accumsan sem lacus nec elit. Vestibulum ac felis vitae odio interdum " +
                        "ullamcorper."
                    },
                }
            },
            {
                "ColNVarcharMax",
                new Dictionary<string, object>
                {
                    {
                        "NVarchar(max)",
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer ut ullamcorper nisl. Nulla iaculis " +
                        "scelerisque dui ut molestie. Suspendisse potenti. In hac habitasse platea dictumst. Maecenas " +
                        "pellentesque ante tortor, vitae pellentesque dolor eleifend sed. Cras a commodo arcu. Nulla " +
                        "convallis vulputate quam, vel lobortis mauris feugiat nec. Nullam tincidunt, sapien eu cursus varius, " +
                        "metus lacus ultrices leo, eu accumsan sem lacus nec elit. Vestibulum ac felis vitae odio interdum " +
                        "ullamcorper."
                    },
                }
            },
        };

        private static readonly List<string> _rawDataShortText = new List<string>
        {
            "Lorem",
            "consectetur",
            "Integer",
            "scelerisque",
            "pellentesque",
            "convallis",
            "metus",
            "ullamcorper.",
        };

        private DataTable LoadData(SqlConnection cn, string tableName)
        {
            DataTable data = new DataTable(TableName);

            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", cn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.FillSchema(data, SchemaType.Source);
                adapter.Fill(data);
            }

            return data;
        }

        private DataTable CreateDataTableDataSource(bool addDataColumns = true)
        {
            DataTable table = CreateBulkInsertDataTable(addDataColumns);
            FillBulkInsertDataTable(table);
            return table;
        }

        private DataTable CreateDataTableShortText()
        {
            DataTable table = CreateBulkInsertDataTableShortText();
            FillBulkInsertDataShortText(table);
            return table;
        }

        private DataTable CreateBulkInsertDataTable(bool addDataColumns)
        {
            DataTable table = new DataTable("data");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ColNote", typeof(string));
            if (addDataColumns)
            {
                table.Columns.Add("ColByte", typeof(byte));
                table.Columns.Add("ColInt32", typeof(int));
                table.Columns.Add("ColInt64", typeof(long));
                table.Columns.Add("ColSingle", typeof(float));
                table.Columns.Add("ColDouble", typeof(double));
                table.Columns.Add("ColDecimal", typeof(decimal));
                table.Columns.Add("ColCurrency", typeof(decimal));
                table.Columns.Add("ColDate", typeof(DateTime));
                table.Columns.Add("ColGuid", typeof(Guid));
                table.Columns.Add("ColBool", typeof(bool));
                table.Columns.Add("ColShortText", typeof(string));
                table.Columns.Add("ColLongText", typeof(string));
                table.Columns.Add("ColNVarcharMax", typeof(string));
            }

            table.PrimaryKey = new DataColumn[] { table.Columns["Id"] };

            return table;
        }

        private void FillBulkInsertDataTable(DataTable table)
        {
            int id = 1;
            foreach (KeyValuePair<string, Dictionary<string, object>> rawItem in _rawData)
            {
                AddBulkInsertDataRow(table, ref id, rawItem.Key, rawItem.Value);
            }
        }

        private void AddBulkInsertDataRow(
            DataTable table,
            ref int id,
            string columnName,
            IDictionary<string, object> columnValues)
        {
            foreach (KeyValuePair<string, object> data in columnValues)
            {
                DataRow row = table.NewRow();

                row["Id"] = id;
                row["ColNote"] = data.Key;
                if (table.Columns.Contains(columnName))
                {
                    row[columnName] = data.Value;
                }
                table.Rows.Add(row);
                id++;
            }
        }

        private DataTable CreateBulkInsertDataTableShortText()
        {
            DataTable table = new DataTable("data");
            DataColumn id = table.Columns.Add("Id", typeof(int));
            id.AutoIncrement = true;
            id.AutoIncrementSeed = 1;
            id.AutoIncrementStep = 1;
            table.Columns.Add("ColValue", typeof(string));
            table.PrimaryKey = new DataColumn[] { table.Columns["Id"] };
            return table;
        }

        private void FillBulkInsertDataShortText(DataTable table)
        {
            foreach (string data in _rawDataShortText)
            {
                DataRow row = table.NewRow();
                row["ColValue"] = data;
                table.Rows.Add(row);
            }
        }

        private IBulkActionDataReader CreateIDataReaderDataSource(bool addDataColumns)
        {
            List<DataItem> data = new List<DataItem>();
            List<string> columnNames = new List<string>(new string[] { "Id", "ColNote" });
            int id = 1;

            foreach (KeyValuePair<string, Dictionary<string, object>> rawItem in _rawData)
            {
                AddBulkInsertDataItem(data, ref id, rawItem.Key, rawItem.Value);
                if (addDataColumns)
                {
                    columnNames.Add(rawItem.Key);
                }
            }

            return new EnumerableDataReader<DataItem>(data, columnNames);
        }

        private IBulkActionDataReader CreateIDataReaderShortText()
            => new EnumerableDataReader<string>(_rawDataShortText, new List<string> { "ColValue" });

        private IBulkActionDataReader CreateDataSourceWithNonExistingColumn()
        {
            return new EnumerableDataReader<NonExistingColumnDataItem>(
                new NonExistingColumnDataItem[] {
                    new NonExistingColumnDataItem() { Id = 1, ColNote = "one", NonExistingColumn = 1 },
                    new NonExistingColumnDataItem() { Id = 2, ColNote = "two", NonExistingColumn = 2 }
                },
                new[] {
                    nameof(NonExistingColumnDataItem.Id),
                    nameof(NonExistingColumnDataItem.ColNote),
                    nameof(NonExistingColumnDataItem.NonExistingColumn)
                }
            );
        }

        private void AddBulkInsertDataItem(
            List<DataItem> data,
            ref int id,
            string columnName,
            IDictionary<string, object> columnValues)
        {
            foreach (KeyValuePair<string, object> value in columnValues)
            {
                DataItem item = new DataItem()
                {
                    Id = id,
                    ColNote = value.Key,
                };
                typeof(DataItem).GetProperty(columnName).SetValue(item, value.Value);
                data.Add(item);
                id++;
            }
        }

        #endregion
    }
}
