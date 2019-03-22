using FluentAssertions;
using Kros.Data;
using Kros.Data.SqlServer;
using Kros.UnitTests;
using System.Collections.Generic;
using System.Data.SqlClient;
using Xunit;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    public class SqlServerIdGeneratorShould : DatabaseTestBase
    {
        #region DatabaseTestBase Overrides

        protected override IEnumerable<string> DatabaseInitScripts =>
            new List<string>() {
                SqlServerIdGenerator.GetIdStoreTableCreationScript(),
                SqlServerIdGenerator.GetStoredProcedureCreationScript()
            };

        #endregion

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
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            using (var cmd = ServerHelper.Connection.CreateCommand())
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
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            using (var cmd = ServerHelper.Connection.CreateCommand())
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
        public void CreateTableAndStoredProcedureForIdGeneratorIfNotExits()
        {
            const string tableName = "IdStore";
            const string procedureName = "spGetNewId";

            using (var helper = new SqlServerTestHelper(BaseConnectionString, BaseDatabaseName))
            {
                HasTable(helper.Connection, tableName).Should().BeFalse();
                HasProcedure(helper.Connection, procedureName).Should().BeFalse();

                var idGenerator = new SqlServerIdGenerator(helper.Connection, "TestTable", 1);
                idGenerator.InitDatabaseForIdGenerator();

                HasTable(helper.Connection, tableName).Should().BeTrue();
                HasProcedure(helper.Connection, procedureName).Should().BeTrue();
            }
        }

        [Fact]
        public void NotThrowWhenCreatingTableAndStoredProcedureForIdGeneratorAndTheyExist()
        {
            const string tableName = "IdStore";
            const string procedureName = "spGetNewId";

            using (var helper = new SqlServerTestHelper(BaseConnectionString, BaseDatabaseName, DatabaseInitScripts))
            {
                HasTable(helper.Connection, tableName).Should().BeTrue();
                HasProcedure(helper.Connection, procedureName).Should().BeTrue();

                var idGenerator = new SqlServerIdGenerator(helper.Connection, "TestTable", 1);
                idGenerator.InitDatabaseForIdGenerator();

                HasTable(helper.Connection, tableName).Should().BeTrue();
                HasProcedure(helper.Connection, procedureName).Should().BeTrue();
            }
        }

        #region Helpers

        private SqlServerIdGeneratorFactory GetFactory()
            => new SqlServerIdGeneratorFactory(ServerHelper.Connection);

        private bool HasTable(SqlConnection connection, string tableName)
        {
            using (ConnectionHelper.OpenConnection(connection))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP 1 1 FROM sys.tables WHERE name='{tableName}' AND type='U'";
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool HasProcedure(SqlConnection connection, string procedureName)
        {
            using (ConnectionHelper.OpenConnection(connection))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP 1 1 FROM sys.procedures WHERE name='{procedureName}' AND type='P'";
                return cmd.ExecuteScalar() != null;
            }
        }

        #endregion
    }
}