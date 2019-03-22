using FluentAssertions;
using Kros.Data.MsAccess;
using Kros.Data.Schema;
using Kros.Data.Schema.MsAccess;
using Kros.UnitTests;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Kros.Utils.UnitTests.Data.Schema
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

        [SkippableFact]
        public void LoadCorrectTableSchemaFromMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectTableSchemaCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void LoadCorrectTableSchemaFromMdb2()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectTableSchemaCore2(helper.Connection);
            }
        }

        [SkippableFact]
        public void LoadCorrectTableSchemaFromAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                LoadCorrectTableSchemaCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void LoadCorrectTableSchemaFromAccdb2()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                LoadCorrectTableSchemaCore2(helper.Connection);
            }
        }

        [SkippableFact]
        public void ReturnNullIfTableDoesNotExistInMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                ReturnNullIfTableDoesNotExistCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void ReturnNullIfTableDoesNotExistInAccdb()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Ace, AccdbFileName))
            {
                ReturnNullIfTableDoesNotExistCore(helper.Connection);
            }
        }

        [SkippableFact]
        public void LoadCorrectIndexesFromMdb()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using (var helper = CreateHelper(ProviderType.Jet, MdbFileName))
            {
                LoadCorrectIndexesCore(helper.Connection);
            }
        }

        [SkippableFact]
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
            actual.Should().BeNull();
        }

        private void LoadCorrectIndexesCore(OleDbConnection cn)
        {
            MsAccessSchemaLoader loader = new MsAccessSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(cn);
            TableSchema table = schema.Tables["IndexesTest"];

            table.PrimaryKey.Should().NotBeNull();
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
            actualColumns.Should().BeEquivalentTo(expectedColumns, "Missing or invalid columns in table.");

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
            msAccessColumn.OleDbType.Should().Be(oleDbType, $"{columnName} should have correct OleDbType.");
            msAccessColumn.AllowNull.Should().Be(allowNull, $"{columnName} should allow NULL.");
            if (defaultValue != null)
            {
                msAccessColumn.DefaultValue.Should().Be(defaultValue, $"{columnName} should have correct default value.");
            }
            if (size.HasValue)
            {
                msAccessColumn.Size.Should().Be(size, $"{columnName} should have correct size.");
            }
            if (precision.HasValue)
            {
                msAccessColumn.Precision.Should().Be(precision, $"{columnName} should have correct precision.");
            }
            if (scale.HasValue)
            {
                msAccessColumn.Scale.Should().Be(scale, $"{columnName} should have correct scale.");
            }
        }

        private static void CheckIndex(IndexSchema index, IndexType indexType, Tuple<string, SortOrder>[] columns)
        {
            index.IndexType.Should().Be(indexType, $"Index {index.Name} should have correct type.");
            index.Columns.Count.Should().Be(columns.Length, $"Index {index.Name} should have correct columns count.");

            IEnumerable<string> expectedColumns = from column in columns select column.Item1;
            IEnumerable<string> actualColumns = from column in index.Columns select column.Name;
            actualColumns.Should().Equal(expectedColumns, $"Index {index.Name} should have correct columns.");

            IEnumerable<SortOrder> expectedOrdering = from column in columns select column.Item2;
            IEnumerable<SortOrder> actualOrdering = from column in index.Columns select column.Order;
            actualOrdering.Should().Equal(expectedOrdering, $"Index {index.Name} should have correct columns ordering.");
        }

        #endregion
    }
}
