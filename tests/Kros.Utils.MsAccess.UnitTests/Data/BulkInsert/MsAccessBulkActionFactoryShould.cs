using FluentAssertions;
using Kros.Data.BulkActions.MsAccess;
using System.Data.OleDb;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data.BulkInsert
{
    public class MsAccessBulkActionFactoryShould
    {
        [Fact]
        public void CreateMsAccessBulkInsertByConnection()
        {
            using (var conn = new OleDbConnection())
            {
                var factory = new MsAccessBulkActionFactory(conn);
                var bulkInsert = factory.GetBulkInsert(null, 10) as MsAccessBulkInsert;

                bulkInsert.CodePage.Should().Be(10);
            }
        }

        [Fact]
        public void CreateMsAccessBulkUpdateByConnection()
        {
            using (var conn = new OleDbConnection())
            {
                var factory = new MsAccessBulkActionFactory(conn);
                var bulkUpdate = factory.GetBulkUpdate() as MsAccessBulkUpdate;

                bulkUpdate.Should().NotBeNull();
            }
        }
    }
}
