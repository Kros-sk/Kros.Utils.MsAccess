using Kros.UnitTests;

namespace Kros.Examples
{
    public class SqlServerTestHelperExamples
    {
        #region Private helpers

        private class FactAttribute
            : System.Attribute
        {
        }

        #endregion

        #region SqlServerTestHelper
        // V connection string-u nie je určená databáza, pretože tá sa automaticky vytvorí s náhodným
        // menom. Na konci práce sa databáza automaticky vymaže.
        private const string BaseConnectionString = "Data Source=SQLSERVER;Integrated Security=True;";

        private const string CreateTestTableScript =
        @"CREATE TABLE [dbo].[TestTable] (
            [Id] [int] NOT NULL,
            [Name] [nvarchar](255) NULL,

            CONSTRAINT [PK_TestTable] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
        ) ON [PRIMARY];";

        [Fact]
        public void DoSomeTestWithDatabase()
        {
            using (var serverHelper = new SqlServerTestHelper(BaseConnectionString, "TestDatabase", CreateTestTableScript))
            {
                // Do tests with connection serverHelper.Connection.
            }
        }
        #endregion
    }
}
