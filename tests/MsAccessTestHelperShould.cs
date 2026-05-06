using Kros.Data.MsAccess;
using Kros.UnitTests;
using Kros.Utils.UnitTests;
using System.Data.OleDb;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests
{
    public class MsAccessTestHelperShould
    {
        [Fact]
        public void CreateNewDatabaseAndCleanUp()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            string databasePath = null;
            using (var helper = new MsAccessTestHelper(ProviderType.Jet))
            {
                Assert.NotNull(helper.Connection);
                databasePath = helper.DatabasePath;
                Assert.NotNull(databasePath);
                Assert.NotEqual(string.Empty, databasePath);
                Assert.True(File.Exists(databasePath));
            }
            Assert.False(File.Exists(databasePath));
        }

        [Fact]
        public void CreateDatabaseWhenDatabasePathIsUsed()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using var helper = new MsAccessTestHelper(ProviderType.Jet, @"./Resources/MsAccessTestHelper.mdb");
            OleDbConnection connection = helper.Connection;

            Assert.NotNull(connection);
        }
    }
}
