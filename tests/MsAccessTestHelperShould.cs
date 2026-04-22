using FluentAssertions;
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
                helper.Connection.Should().NotBeNull();
                databasePath = helper.DatabasePath;
                databasePath.Should().NotBeNullOrEmpty();
                File.Exists(databasePath).Should().BeTrue();
            }
            File.Exists(databasePath).Should().BeFalse();
        }

        [Fact]
        public void CreateDatabaseWhenDatabasePathIsUsed()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            using var helper = new MsAccessTestHelper(ProviderType.Jet, @"./Resources/MsAccessTestHelper.mdb");
            OleDbConnection connection = helper.Connection;

            connection.Should().NotBeNull();
        }
    }
}
