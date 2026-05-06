using Kros.Data.MsAccess;
using Kros.Data.Schema;
using Kros.Data.Schema.MsAccess;
using Kros.UnitTests;
using Kros.Utils.UnitTests;
using System.Data.OleDb;
using System.Reflection;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data.Schema
{
    public class MsAccessSchemaLoaderShould
    {
        #region Constants

        private const string AccdbFileName = "MsAccessSchema.accdb";
        private const string MdbFileName = "MsAccessSchema.mdb";

        private const string TestSchemaTableName = "SchemaTest";
        private const string InvalidTableName = "NonExistingTable";

        #endregion

        #region Tests

        [Fact]
        public void LoadCorrectTableSchemaFromMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectTableSchemaCore(helper.Connection);
            }
        }

        [Fact]
        public void LoadCorrectTableSchemaFromMdb2()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectTableSchemaCore2(helper.Connection);
            }
        }

        [Fact]
        public void LoadCorrectTableSchemaFromAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                LoadCorrectTableSchemaCore(helper.Connection);
            }
        }

        [Fact]
        public void LoadCorrectTableSchemaFromAccdb2()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                LoadCorrectTableSchemaCore2(helper.Connection);
            }
        }

        [Fact]
        public void ReturnNullIfTableDoesNotExistInMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                ReturnNullIfTableDoesNotExistCore(helper.Connection);
            }
        }

        [Fact]
        public void ReturnNullIfTableDoesNotExistInAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                ReturnNullIfTableDoesNotExistCore(helper.Connection);
            }
        }

        [Fact]
        public void LoadCorrectIndexesFromMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectIndexesCore(helper.Connection);
            }
        }

        [Fact]
        public void LoadCorrectIndexesFromAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                LoadCorrectIndexesCore(helper.Connection);
            }
        }

        #endregion

        #region Helpers

        private MsAccessTestHelper CreateHelper(ProviderType provider, string fileName)
        {
            string resourceName = Helpers.RootNamespaceResources + "." + fileName;
            Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return new MsAccessTestHelper(provider, sourceStream);
        }

        private void LoadCorrectTableSchemaCore(OleDbConnection cn)
        {
            MsAccessSchemaLoader loader = new MsAccessSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(cn);

            CheckTableSchema(schema.Tables[TestSchemaTableName]);
        }

        private void LoadCorrectTableSchemaCore2(OleDbConnection cn)
        {
            MsAccessSchemaLoader loader = new MsAccessSchemaLoader();

            CheckTableSchema(loader.LoadTableSchema(cn, TestSchemaTableName));
        }

        private void ReturnNullIfTableDoesNotExistCore(OleDbConnection cn)
        {
            MsAccessSchemaLoader loader = new MsAccessSchemaLoader();

            TableSchema actual = loader.LoadTableSchema(cn, InvalidTableName);
            Assert.Null(actual);
        }

        private void LoadCorrectIndexesCore(OleDbConnection cn)
        {
            MsAccessSchemaLoader loader = new MsAccessSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(cn);
            TableSchema table = schema.Tables["IndexesTest"];

            Assert.NotNull(table.PrimaryKey);
            CheckIndex(table.PrimaryKey, IndexType.PrimaryKey,
                new Tuple<string, SortOrder>[] { Tuple.Create("Id", SortOrder.Ascending) });
            CheckIndex(table.Indexes["I_Index"], IndexType.Index, new Tuple<string, SortOrder>[] {
                Tuple.Create("ColIndex1", SortOrder.Ascending),
                Tuple.Create("ColIndex2Desc", SortOrder.Descending),
                Tuple.Create("ColIndex3", SortOrder.Ascending)
            });
            CheckIndex(table.Indexes["I_UniqueIndex"], IndexType.UniqueKey,
                new Tuple<string, SortOrder>[] { Tuple.Create("ColUniqueIndex", SortOrder.Ascending) });
        }

        private static void CheckTableSchema(TableSchema table)
        {
            string[] expectedColumns = { "ColByte", "ColInt32", "ColInt64", "ColInt64NotNull", "ColSingle", "ColDouble",
                "ColDecimal", "ColVarChar", "ColVarCharNotNull", "ColText", "ColDateTime", "ColBoolean", "ColGuid" };

            IEnumerable<string> actualColumns = from column in table.Columns select column.Name;
            Assert.Equal(expectedColumns, actualColumns);

            CheckColumnSchema(table.Columns["ColByte"], OleDbType.UnsignedTinyInt, null, true);
            CheckColumnSchema(table.Columns["ColInt32"], OleDbType.SmallInt, (short)32, true);
            CheckColumnSchema(table.Columns["ColInt64"], OleDbType.Integer, 64, true);
            CheckColumnSchema(table.Columns["ColInt64NotNull"], OleDbType.Integer, null, false);
            CheckColumnSchema(table.Columns["ColSingle"], OleDbType.Single, (float)123.456, true, null, null, null);
            CheckColumnSchema(table.Columns["ColDouble"], OleDbType.Double, (double)654.321, true, null, null, null);
            CheckColumnSchema(table.Columns["ColDecimal"], OleDbType.Numeric, (double)1234.5678, true, null, 18, 5);
            CheckColumnSchema(table.Columns["ColVarChar"], OleDbType.WChar, null, true, 100, null, null);
            CheckColumnSchema(table.Columns["ColVarCharNotNull"], OleDbType.WChar, "Lorem ipsum", false, 200, null, null);
            CheckColumnSchema(table.Columns["ColText"], OleDbType.WChar, null, true);
            CheckColumnSchema(table.Columns["ColDateTime"], OleDbType.Date, new DateTime(1978, 12, 10), true);
            CheckColumnSchema(table.Columns["ColBoolean"], OleDbType.Boolean, true, false);
            CheckColumnSchema(table.Columns["ColGuid"], OleDbType.Guid, null, true);
        }

        private static void CheckColumnSchema(
            ColumnSchema column, OleDbType oleDbType, object defaultValue, bool allowNull)
        {
            CheckColumnSchema(column, oleDbType, defaultValue, allowNull, null, null, null);
        }

        private static void CheckColumnSchema(
            ColumnSchema column,
            OleDbType oleDbType,
            object defaultValue,
            bool allowNull,
            int? size,
            byte? precision,
            byte? scale)
        {
            MsAccessColumnSchema msAccessColumn = (MsAccessColumnSchema)column;
            string columnName = msAccessColumn.Name;
            Assert.Equal(oleDbType, msAccessColumn.OleDbType);
            Assert.Equal(allowNull, msAccessColumn.AllowNull);
            if (defaultValue != null)
            {
                Assert.Equal(defaultValue, msAccessColumn.DefaultValue);
            }
            if (size.HasValue)
            {
                Assert.Equal(size, msAccessColumn.Size);
            }
            if (precision.HasValue)
            {
                Assert.Equal(precision, msAccessColumn.Precision);
            }
            if (scale.HasValue)
            {
                Assert.Equal(scale, msAccessColumn.Scale);
            }
        }

        private static void CheckIndex(IndexSchema index, IndexType indexType, Tuple<string, SortOrder>[] columns)
        {
            Assert.Equal(indexType, index.IndexType);
            Assert.Equal(columns.Length, index.Columns.Count);

            IEnumerable<string> expectedColumns = from column in columns select column.Item1;
            IEnumerable<string> actualColumns = from column in index.Columns select column.Name;
            Assert.Equal(expectedColumns, actualColumns);

            IEnumerable<SortOrder> expectedOrdering = from column in columns select column.Item2;
            IEnumerable<SortOrder> actualOrdering = from column in index.Columns select column.Order;
            Assert.Equal(expectedOrdering, actualOrdering);
        }

        #endregion
    }
}
