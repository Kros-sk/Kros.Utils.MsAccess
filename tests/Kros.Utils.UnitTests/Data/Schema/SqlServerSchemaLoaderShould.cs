using FluentAssertions;
using Kros.Data.Schema;
using Kros.Data.Schema.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Kros.Utils.UnitTests.Data.Schema
{
    public class SqlServerSchemaLoaderShould : DatabaseTestBase
    {
        #region Constants

        private const string TestSchemaTableName = "SchemaTest";

        private static readonly string[] _databaseInitScripts = new string[]
        {
            CreateTable_SchemaTest,
            CreateTable_IndexesTest,
            CreateTable_ForeignKeyTest_ParentTable,
            CreateTable_ForeignKeyTest_ChildTableNoAction,
            CreateTable_ForeignKeyTest_ChildTableSetDefault,
            CreateTable_ForeignKeyTest_ChildTableSetNull,
            CreateTable_ForeignKeyTest_ChildTableCascade
        };

        private const string CreateTable_SchemaTest =
@"CREATE TABLE [dbo].[SchemaTest] (
    [ColByte] [tinyint] NULL,
    [ColInt32] [int] NULL,
    [ColInt64] [bigint] NULL,
    [ColInt64NotNull] [bigint] NOT NULL,

    [ColSingle] [real] NULL,
    [ColDouble] [float] NULL,
    [ColDecimal] [decimal](18, 5) NULL,
    [ColCurrency] [money] NULL,

    [ColVarChar] [nvarchar](100) NULL,
    [ColVarCharNotNull] [nvarchar](200) NOT NULL,
    [ColText] [ntext] NULL,
    [ColNVarcharMax] [nvarchar](max) NULL,

    [ColDate] [date] NULL,
    [ColDateTime] [datetime] NULL,
    [ColDateTime2] [datetime2](7) NULL,
    [ColDateTimeOffset] [datetimeoffset] NULL,
    [ColDateTimeOffset2] [datetimeoffset] NULL,
    [ColSmallDateTime] [smalldatetime] NULL,

    [ColBoolean] [bit] NOT NULL,
    [ColGuid] [uniqueidentifier] NULL
) ON[PRIMARY];

ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColInt32] DEFAULT((32)) FOR [ColInt32];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColInt64] DEFAULT((64)) FOR [ColInt64];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColSingle] DEFAULT((123.456)) FOR [ColSingle];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDouble] DEFAULT((654.321)) FOR [ColDouble];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDecimal] DEFAULT((1234.5678)) FOR [ColDecimal];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColCurrency] DEFAULT((1234.5678)) FOR [ColCurrency];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColVarCharNotNull] DEFAULT(N'Lorem ipsum') FOR [ColVarCharNotNull];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDate] DEFAULT('1978-12-10') FOR [ColDate];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDateTime] DEFAULT('1978-12-10 06:30:00') FOR [ColDateTime];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDateTime2] DEFAULT('1978-12-10 06:30:00.1234') FOR [ColDateTime2];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDateTimeOffset] DEFAULT('1978-12-10 06:30:00.123400 +01:30') FOR [ColDateTimeOffset];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColDateTimeOffset2] DEFAULT('1978-12-10 06:30:00.123400 -01:30') FOR [ColDateTimeOffset2];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColSmallDateTime] DEFAULT('1978-12-10 06:30:00') FOR [ColSmallDateTime];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColBoolean] DEFAULT('TRUE') FOR [ColBoolean];
ALTER TABLE[dbo].[SchemaTest] ADD CONSTRAINT[DF_SchemaTest_ColGuid] DEFAULT('01234567-89ab-cdef-0123-456789abcdef') FOR [ColGuid];
";

        private const string CreateTable_IndexesTest =
            @"CREATE TABLE [dbo].[IndexesTest] (
    [Id] [int] NOT NULL,
    [ColIndex1] [int] NULL,
    [ColIndex2Desc] [int] NULL,
    [ColIndex3] [nvarchar](255) NULL,
    [ColUniqueIndex] [int] NULL,

    CONSTRAINT [PK_IndexesTest_PK] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];

CREATE NONCLUSTERED INDEX [I_Index] ON [dbo].[IndexesTest] ([ColIndex1] ASC, [ColIndex2Desc] DESC, [ColIndex3] ASC) ON [PRIMARY];
CREATE UNIQUE NONCLUSTERED INDEX [I_UniqueIndex] ON [dbo].[IndexesTest] ([ColUniqueIndex] ASC) ON [PRIMARY];
";

        private const string CreateTable_ForeignKeyTest_ParentTable =
            @"CREATE TABLE [dbo].[ParentTable] (
    [Id] [int] NOT NULL,
    [ColName] [nvarchar](50) NULL,

    CONSTRAINT [PK_ParentTable] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];
";

        private const string CreateTable_ForeignKeyTest_ChildTableNoAction =
            @"CREATE TABLE [dbo].[ChildTableNoAction] (
    [Id] [int] NOT NULL,
    [ParentId] [int] NULL,
    [ColName] [nvarchar](50) NULL,

    CONSTRAINT [PK_ChildTableNoAction] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];

ALTER TABLE [dbo].[ChildTableNoAction] WITH CHECK ADD CONSTRAINT [FK_ChildTableNoAction_ParentTable]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[ParentTable] ([Id]);

ALTER TABLE [dbo].[ChildTableNoAction] CHECK CONSTRAINT [FK_ChildTableNoAction_ParentTable];
";

        private const string CreateTable_ForeignKeyTest_ChildTableSetNull =
            @"CREATE TABLE [dbo].[ChildTableSetNull] (
    [Id] [int] NOT NULL,
    [ParentId] [int] NULL,
    [ColName] [nvarchar](50) NULL,

    CONSTRAINT [PK_ChildTableSetNull] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];

ALTER TABLE [dbo].[ChildTableSetNull] WITH CHECK ADD CONSTRAINT [FK_ChildTableSetNull_ParentTable]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[ParentTable] ([Id])
    ON UPDATE SET NULL
    ON DELETE SET NULL;

ALTER TABLE [dbo].[ChildTableSetNull] CHECK CONSTRAINT [FK_ChildTableSetNull_ParentTable];
";

        private const string CreateTable_ForeignKeyTest_ChildTableSetDefault =
            @"CREATE TABLE [dbo].[ChildTableSetDefault] (
    [Id] [int] NOT NULL,
    [ParentId] [int] NULL,
    [ColName] [nvarchar](50) NULL,

    CONSTRAINT [PK_ChildTableDefault] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [dbo].[ChildTableSetDefault] WITH CHECK ADD CONSTRAINT [FK_ChildTableSetDefault_ParentTable]
    FOREIGN KEY([ParentId])
    REFERENCES [dbo].[ParentTable] ([Id])
    ON UPDATE SET DEFAULT
    ON DELETE SET DEFAULT;

ALTER TABLE [dbo].[ChildTableSetDefault] CHECK CONSTRAINT [FK_ChildTableSetDefault_ParentTable];
";

        private const string CreateTable_ForeignKeyTest_ChildTableCascade =
            @"CREATE TABLE [dbo].[ChildTableCascade] (
    [Id] [int] NOT NULL,
    [ParentId] [int] NULL,
    [ColName] [nvarchar](50) NULL,

    CONSTRAINT [PK_ChildTableCascade] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]

) ON [PRIMARY];

ALTER TABLE [dbo].[ChildTableCascade] WITH CHECK ADD CONSTRAINT [FK_ChildTableCascade_ParentTable]
    FOREIGN KEY([ParentId])
    REFERENCES [dbo].[ParentTable] ([Id])
    ON UPDATE CASCADE
    ON DELETE CASCADE;

ALTER TABLE [dbo].[ChildTableCascade] CHECK CONSTRAINT [FK_ChildTableCascade_ParentTable];
";

        #endregion

        #region DatabaseTestBase Overrides

        protected override IEnumerable<string> DatabaseInitScripts => _databaseInitScripts;

        #endregion

        #region Tests

        [Fact]
        public void LoadCorrectTableSchema()
        {
            SqlServerSchemaLoader loader = new SqlServerSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(ServerHelper.Connection);
            CheckTableSchema(schema.Tables[TestSchemaTableName]);
        }

        [Fact]
        public void LoadCorrectTableSchema2()
        {
            SqlServerSchemaLoader loader = new SqlServerSchemaLoader();

            CheckTableSchema(loader.LoadTableSchema(ServerHelper.Connection, TestSchemaTableName));
        }

        [Fact]
        public void ReturnNullIfTableDoesNotExist()
        {
            SqlServerSchemaLoader loader = new SqlServerSchemaLoader();
            TableSchema actual = loader.LoadTableSchema(ServerHelper.Connection, "NonExistingTable");

            actual.Should().BeNull();
        }

        [Fact]
        public void LoadCorrectIndexes()
        {
            SqlServerSchemaLoader loader = new SqlServerSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(ServerHelper.Connection);
            TableSchema table = schema.Tables["IndexesTest"];

            table.PrimaryKey.Should().NotBeNull();
            table.PrimaryKey.Name.Should().Be("PK_IndexesTest_PK", "Primary key does not have correct name.");
            CheckIndex(table.PrimaryKey, IndexType.PrimaryKey,
                new Tuple<string, SortOrder>[] { Tuple.Create("Id", SortOrder.Ascending) });
            CheckIndex(table.Indexes["I_Index"], IndexType.Index, new Tuple<string, SortOrder>[]
            {
                Tuple.Create("ColIndex1", SortOrder.Ascending),
                    Tuple.Create("ColIndex2Desc", SortOrder.Descending),
                    Tuple.Create("ColIndex3", SortOrder.Ascending)
            });
            CheckIndex(table.Indexes["I_UniqueIndex"], IndexType.UniqueKey,
                new Tuple<string, SortOrder>[]
                {
                    Tuple.Create("ColUniqueIndex", SortOrder.Ascending)
                });
        }

        [Fact]
        public void LoadCorrectForeignKeys()
        {
            SqlServerSchemaLoader loader = new SqlServerSchemaLoader();
            DatabaseSchema schema = loader.LoadSchema(ServerHelper.Connection);
            TableSchema parentTable = schema.Tables["ParentTable"];
            TableSchema childTableNoAction = schema.Tables["ChildTableNoAction"];
            TableSchema childTableSetNull = schema.Tables["ChildTableSetNull"];
            TableSchema childTableSetDefault = schema.Tables["ChildTableSetDefault"];
            TableSchema childTableCascade = schema.Tables["ChildTableCascade"];

            childTableNoAction.ForeignKeys.Count.Should().Be(1, "Tabuľka ChildTableNoAction by mala mať jeden cudzí kľúč.");
            CheckForeignKey(childTableNoAction.ForeignKeys[0], "FK_ChildTableNoAction_ParentTable", ForeignKeyRule.NoAction);

            childTableSetNull.ForeignKeys.Count.Should().Be(1, "Tabuľka ChildTableSetNull by mala mať jeden cudzí kľúč.");
            CheckForeignKey(childTableSetNull.ForeignKeys[0], "FK_ChildTableSetNull_ParentTable", ForeignKeyRule.SetNull);

            childTableSetDefault.ForeignKeys.Count.Should().Be(1, "Tabuľka ChildTableSetDefault by mala mať jeden cudzí kľúč.");
            CheckForeignKey(childTableSetDefault.ForeignKeys[0], "FK_ChildTableSetDefault_ParentTable", ForeignKeyRule.SetDefault);

            childTableCascade.ForeignKeys.Count.Should().Be(1, "Tabuľka ChildTableCascade by mala mať jeden cudzí kľúč.");
            CheckForeignKey(childTableCascade.ForeignKeys[0], "FK_ChildTableCascade_ParentTable", ForeignKeyRule.Cascade);
        }

        #endregion

        #region Helpers

        private static void CheckTableSchema(TableSchema table)
        {
            string[] expectedColumns = {
                "ColByte",
                "ColInt32",
                "ColInt64",
                "ColInt64NotNull",
                "ColSingle",
                "ColDouble",
                "ColDecimal",
                "ColCurrency",
                "ColVarChar",
                "ColVarCharNotNull",
                "ColText",
                "ColNVarcharMax",
                "ColDate",
                "ColDateTime",
                "ColDateTime2",
                "ColDateTimeOffset",
                "ColDateTimeOffset2",
                "ColSmallDateTime",
                "ColBoolean",
                "ColGuid"
            };

            IEnumerable<string> actualColumns = from column in table.Columns select column.Name;
            actualColumns.Should().BeEquivalentTo(expectedColumns, "Missing or invalid columns in table.");

            CheckColumnSchema(table.Columns["ColByte"], SqlDbType.TinyInt, null, true);
            CheckColumnSchema(table.Columns["ColInt32"], SqlDbType.Int, 32, true);
            CheckColumnSchema(table.Columns["ColInt64"], SqlDbType.BigInt, 64L, true);
            CheckColumnSchema(table.Columns["ColInt64NotNull"], SqlDbType.BigInt, null, false);
            CheckColumnSchema(table.Columns["ColSingle"], SqlDbType.Real, (float)123.456, true, null, 24, 0);
            CheckColumnSchema(table.Columns["ColDouble"], SqlDbType.Float, (double)654.321, true, null, 53, 0);
            CheckColumnSchema(table.Columns["ColDecimal"], SqlDbType.Decimal, (decimal)1234.5678, true, null, 18, 5);
            CheckColumnSchema(table.Columns["ColCurrency"], SqlDbType.Money, (decimal)1234.5678, true, null, null, null);
            CheckColumnSchema(table.Columns["ColVarChar"], SqlDbType.NVarChar, null, true, 100, null, null);
            CheckColumnSchema(table.Columns["ColVarCharNotNull"], SqlDbType.NVarChar, "Lorem ipsum", false, 200, null, null);
            CheckColumnSchema(table.Columns["ColText"], SqlDbType.NText, null, true);
            CheckColumnSchema(table.Columns["ColNVarcharMax"], SqlDbType.NVarChar, null, true);

            CheckColumnSchema(table.Columns["ColDate"], SqlDbType.Date, new DateTime(1978, 12, 10), true);

            var dateTimeDefault = new DateTime(1978, 12, 10, 6, 30, 0);
            CheckColumnSchema(table.Columns["ColDateTime"], SqlDbType.DateTime, dateTimeDefault, true);
            CheckColumnSchema(table.Columns["ColSmallDateTime"], SqlDbType.SmallDateTime, dateTimeDefault, true);

            // DateTime and DateTimeOffset do not accept value for milliseconds greater than 999 or fractional milliseconds,
            // so we need to add them this way. 1 ms = 10 000 ticks.
            dateTimeDefault = dateTimeDefault.AddTicks(1234000);
            CheckColumnSchema(table.Columns["ColDateTime2"], SqlDbType.DateTime2, dateTimeDefault, true);
            CheckColumnSchema(table.Columns["ColDateTimeOffset"], SqlDbType.DateTimeOffset, new DateTimeOffset(dateTimeDefault, TimeSpan.FromMinutes(90)), true);
            CheckColumnSchema(table.Columns["ColDateTimeOffset2"], SqlDbType.DateTimeOffset, new DateTimeOffset(dateTimeDefault, TimeSpan.FromMinutes(-90)), true);

            CheckColumnSchema(table.Columns["ColBoolean"], SqlDbType.Bit, true, false);
            CheckColumnSchema(table.Columns["ColGuid"], SqlDbType.UniqueIdentifier, new Guid("01234567-89ab-cdef-0123-456789abcdef"), true);
        }

        private static void CheckColumnSchema(ColumnSchema column, SqlDbType sqlDbType, object defaultValue, bool allowNull)
            => CheckColumnSchema(column, sqlDbType, defaultValue, allowNull, null, null, null);

        private static void CheckColumnSchema(
            ColumnSchema column,
            SqlDbType sqlDbType,
            object defaultValue,
            bool allowNull,
            int? size,
            byte? precision,
            byte? scale)
        {
            SqlServerColumnSchema sqlServerColumn = (SqlServerColumnSchema)column;
            string columnName = sqlServerColumn.Name;
            sqlServerColumn.SqlDbType.Should().Be(sqlDbType, $"{columnName} should have correct SqlDbType.");
            sqlServerColumn.AllowNull.Should().Be(allowNull, $"{columnName} should allow NULL.");
            if (defaultValue != null)
            {
                sqlServerColumn.DefaultValue.Should().Be(defaultValue, $"{columnName} should have correct default value.");
            }
            if (size.HasValue)
            {
                sqlServerColumn.Size.Should().Be(size, $"{columnName} should have correct size.");
            }
            if (precision.HasValue)
            {
                sqlServerColumn.Precision.Should().Be(precision, $"{columnName} should have correct precision.");
            }
            if (scale.HasValue)
            {
                sqlServerColumn.Scale.Should().Be(scale, $"{columnName} should have correct scale.");
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

        private static void CheckForeignKey(ForeignKeySchema foreignKey, string foreignKeyName, ForeignKeyRule rule)
        {
            foreignKey.Name.Should().Be(foreignKeyName, "Meno cudzieho kľúča musí byť správne.");
            foreignKey.PrimaryKeyTableName.Should().Be("ParentTable", "Meno tabuľky s primárnym kľúčom musí byť správne.");
            foreignKey.PrimaryKeyTableColumns.Should().Equal(new string[] { "Id" });
            foreignKey.ForeignKeyTableColumns.Should().Equal(new string[] { "ParentId" });
            foreignKey.DeleteRule.Should().Be(rule, "Pravidlo pri vymazaní (DELETE RULE) musí byť správne.");
            foreignKey.UpdateRule.Should().Be(rule, "Pravidlo pri vymazaní (UPDATE RULE) musí byť správne.");
        }

        #endregion
    }
}