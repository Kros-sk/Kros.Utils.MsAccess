using FluentAssertions;
using Kros.Data.MsAccess;
using Kros.UnitTests;
using System.IO;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests
{
    public class MsAccessTestHelperShould
    {
        [Fact]
        public void CreateNewDatabaseAndCleanUp()
        {
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
            using (var helper = new MsAccessTestHelper(ProviderType.Jet, @"./Resources/MsAccessTestHelper.mdb"))
            {
                var connection = helper.Connection;

                connection.Should().NotBeNull();
            }
        }
    }
}
