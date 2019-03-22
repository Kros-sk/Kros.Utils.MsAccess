using FluentAssertions;
using Kros.Data.BulkActions;
using Kros.Data.SqlServer;
using System;
using System.Data.SqlClient;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class BulkActionFactoriesShould
    {
        [Fact]
        public void GetFactoryByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = BulkActionFactories.GetFactory(conn);

                factory.Should().NotBeNull();
            }
        }

        [Fact]
        public void GetFactoryByAdoClientName()
        {
            var factory = BulkActionFactories.GetFactory("connectionstring", SqlServerDataHelper.ClientId);

            factory.Should().NotBeNull();
        }

        [Fact]
        public void ThrowExceptionWhenConnectionIsNotRegistered()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = BulkActionFactories.GetFactory(conn); };

                action.Should().Throw<InvalidOperationException>()
                    .WithMessage($"*{typeof(CustomConnection).FullName}*");
            }
        }

        [Fact]
        public void ThrowExceptionWhenAdoClientNameIsNotRegistered()
        {
            Action action = () => { var factory = BulkActionFactories.GetFactory("constring", "System.Data.CustomClient"); };

            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"*System.Data.CustomClient*");
        }
    }
}
