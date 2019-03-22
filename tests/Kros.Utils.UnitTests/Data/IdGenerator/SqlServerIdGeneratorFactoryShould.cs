using FluentAssertions;
using Kros.Data.SqlServer;
using System.Data.SqlClient;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class SqlServerIdGeneratorFactoryShould
    {
        [Fact]
        public void CreateSqlServerIdGeneratorByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = new SqlServerIdGeneratorFactory(conn);
                var generator = factory.GetGenerator("Person", 150) as SqlServerIdGenerator;

                generator.TableName.Should().Be("Person");
                generator.BatchSize.Should().Be(150);
            }
        }
    }
}
