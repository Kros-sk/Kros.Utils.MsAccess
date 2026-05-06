using Kros.Data.MsAccess;
using System.Data.OleDb;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data
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

                Assert.Equal("Person", generator.TableName);
                Assert.Equal(10, generator.BatchSize);
            }
        }
    }
}
