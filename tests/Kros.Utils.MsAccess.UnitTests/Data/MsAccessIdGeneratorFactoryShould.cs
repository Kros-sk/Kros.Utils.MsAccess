using FluentAssertions;
using Kros.Data.MsAccess;
using System.Data.OleDb;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class MsAccessIdGeneratorFactoryShould
    {
        [Fact]
        public void CreateMsAccessIdGeneratorByConnection()
        {
            using (var conn = new OleDbConnection())
            {
                var factory = new MsAccessIdGeneratorFactory(conn);
                var generator = factory.GetGenerator("Person", 10) as MsAccessIdGenerator;

                generator.TableName.Should().Be("Person");
                generator.BatchSize.Should().Be(10);
            }
        }
    }
}
