using FluentAssertions;
using Kros.Data.MsAccess;
using Kros.Data.Schema.MsAccess;
using Kros.UnitTests;
using Kros.Utils.UnitTests;
using System;
using System.IO;
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
                    idGenerator.GetNext().Should().Be(i + 1);
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
                    idGenerator.GetNext().Should().Be(i + 1);
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
                    idGenerator.GetNext().Should().Be(10 + i + 1);
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
                    idGenerator.GetNext().Should().Be(10 + i + 1);
                }
            }
        }

        [Fact]
        public void MultipleGenerateIdsForTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                idGenerator.GetNext().Should().Be(1);

                using (var nextGenerator = GetFactory().GetGenerator("People", 3))
                {
                    nextGenerator.GetNext().Should().Be(2);

                    idGenerator.GetNext().Should().Be(5);

                    nextGenerator.GetNext().Should().Be(3);

                    nextGenerator.GetNext().Should().Be(4);

                    nextGenerator.GetNext().Should().Be(6);
                }

                idGenerator.GetNext().Should().Be(9);
            }
        }

        [Fact]
        public void GenerateIdsForMoreTable()
        {
            using (var idGenerator = GetFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    idGenerator.GetNext().Should().Be(i + 1);

                    using (var addressIdGenerator = GetFactory().GetGenerator("Addresses", 5))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            addressIdGenerator.GetNext().Should().Be(j + 5 * i + 1);
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
                schemaLoader.LoadTableSchema(helper.Connection, tableName).Should().BeNull();

                var idGenerator = new MsAccessIdGenerator(helper.Connection, "TestTable", 1);
                idGenerator.InitDatabaseForIdGenerator();

                schemaLoader.LoadTableSchema(helper.Connection, tableName).Should().NotBeNull();
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
                schemaLoader.LoadTableSchema(helper.Connection, tableName).Should().NotBeNull();

                idGenerator.InitDatabaseForIdGenerator();
                schemaLoader.LoadTableSchema(helper.Connection, tableName).Should().NotBeNull();
            }
        }

        #region Helpers

        private MsAccessTestHelper CreateHelper()
        {
            string resourceName = Helpers.RootNamespaceResources + ".MsAccessIdGenerator.mdb";
            Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return new MsAccessTestHelper(ProviderType.Jet, sourceStream);
        }

        private MsAccessTestHelper CreateHelperWithEmptyDatabase()
        {
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
