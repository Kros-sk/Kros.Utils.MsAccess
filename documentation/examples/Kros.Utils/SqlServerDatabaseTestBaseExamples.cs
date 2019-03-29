using System;

namespace Kros.Utils.Examples
{
    public class FactAttribute
        : Attribute
    {
    }

    #region SqlServerDatabaseTestBase

    public class SomeDatabaseTests
        : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        protected override string BaseConnectionString => "Data Source=TESTSQLSERVER;Integrated Security=True";

        [Fact]
        public void Test1()
        {
            using (var cmd = ServerHelper.Connection.CreateCommand())
            {
                // Use cmd to execute queries.
            }
        }

        [Fact]
        public void Test2()
        {
        }
    }

    #endregion
}
