using Kros.Data.MsAccess;
using Kros.Data.Schema.MsAccess;
using Kros.UnitTests;
using Kros.Utils.UnitTests;
using System.Reflection;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data
{
    public class MsAccessIdGeneratorShould : IDisposable
    {
        private MsAccessTestHelper _helper;

        public MsAccessIdGeneratorShould()
        {
            _helper = CreateHelper();
            _helper.Connection.Open();
        }

        [Fact]
        public void GenerateIdsForTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(i + 1, idGenerator.GetNext());
                }
            }
        }

        [Fact]
        public void GenerateBatchIdsForTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People", 10))
            {
                for (int i = 0; i < 15; i++)
                {
                    Assert.Equal(i + 1, idGenerator.GetNext());
                }
            }
        }

        [Fact]
        public void GenerateIdsForTableWhenDataExists()
        {
            using (var cmd = _helper.Connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO IdStore VALUES ('People', 10)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(10 + i + 1, idGenerator.GetNext());
                }
            }
        }

        [Fact]
        public void GenerateBatchIdsForTableWhenDataExists()
        {
            using (var cmd = _helper.Connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO IdStore VALUES ('People', 10)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            using (var idGenerator = GetFactory().GetGenerator("People", 10))
            {
                for (int i = 0; i < 15; i++)
                {
                    Assert.Equal(10 + i + 1, idGenerator.GetNext());
                }
            }
        }

        [Fact]
        public void MultipleGenerateIdsForTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                Assert.Equal(1, idGenerator.GetNext());

                using (var nextGenerator = GetFactory().GetGenerator("People", 3))
                {
                    Assert.Equal(2, nextGenerator.GetNext());
                    Assert.Equal(5, idGenerator.GetNext());
                    Assert.Equal(3, nextGenerator.GetNext());
                    Assert.Equal(4, nextGenerator.GetNext());
                    Assert.Equal(6, nextGenerator.GetNext());
                }

                Assert.Equal(9, idGenerator.GetNext());
            }
        }

        [Fact]
        public void GenerateIdsForMoreTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(i + 1, idGenerator.GetNext());

                    using (var addressIdGenerator = GetFactory().GetGenerator("Addresses", 5))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Assert.Equal(j + 5 * i + 1, addressIdGenerator.GetNext());
                        }
                    }
                }
            }
        }

        [Fact]
        public void CreateTableForIdGeneratorIfNotExits()
        {
            const string tableName = "IdStore";

            using (var helper = CreateHelperWithEmptyDatabase())
            {
                var schemaLoader = new MsAccessSchemaLoader();
                Assert.Null(schemaLoader.LoadTableSchema(helper.Connection, tableName));

                var idGenerator = new MsAccessIdGenerator(helper.Connection, "TestTable", 1);
                idGenerator.InitDatabaseForIdGenerator();

                Assert.NotNull(schemaLoader.LoadTableSchema(helper.Connection, tableName));
            }
        }

        [Fact]
        public void NotThrowWhenCreatingTableAndStoredProcedureForIdGeneratorAndTheyExist()
        {
            const string tableName = "IdStore";

            using (var helper = CreateHelperWithEmptyDatabase())
            {
                var schemaLoader = new MsAccessSchemaLoader();

                var idGenerator = new MsAccessIdGenerator(helper.Connection, "TestTable", 1);
                idGenerator.InitDatabaseForIdGenerator();
                Assert.NotNull(schemaLoader.LoadTableSchema(helper.Connection, tableName));

                idGenerator.InitDatabaseForIdGenerator();
                Assert.NotNull(schemaLoader.LoadTableSchema(helper.Connection, tableName));
            }
        }

        #region Helpers

        private MsAccessTestHelper CreateHelper()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();

            string resourceName = Helpers.RootNamespaceResources + ".MsAccessIdGenerator.mdb";
            Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return new MsAccessTestHelper(ProviderType.Jet, sourceStream);
        }

        private MsAccessTestHelper CreateHelperWithEmptyDatabase()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();

            string resourceName = Helpers.RootNamespaceResources + ".MsAccessSchema.mdb";
            Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return new MsAccessTestHelper(ProviderType.Jet, sourceStream);
        }

        private MsAccessIdGeneratorFactory GetFactory()
            => new MsAccessIdGeneratorFactory(_helper.Connection);

        #endregion

        public void Dispose()
        {
            _helper.Dispose();
        }
    }
}
