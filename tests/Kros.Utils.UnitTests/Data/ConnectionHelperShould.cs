using FluentAssertions;
using Kros.Data;
using System.Data;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class ConnectionHelperShould
        : DatabaseTestBase
    {
        [Fact]
        public void OpenConnectionOnStartAndCloseItAtTheEnd()
        {
            ServerHelper.Connection.Close();

            ServerHelper.Connection.State.Should().Be(ConnectionState.Closed);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            }
            ServerHelper.Connection.State.Should().Be(ConnectionState.Closed);
        }

        [Fact]
        public void KeepConnectionOpenedAtTheEnd()
        {
            if (!ServerHelper.Connection.IsOpened())
            {
                ServerHelper.Connection.Open();
            }

            ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            }
            ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
        }
    }
}
