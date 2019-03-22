namespace Kros.Utils.UnitTests
{
    /// <summary>
    /// Základná trieda pre databázové integračné testy.
    /// </summary>
    public class DatabaseTestBase
        : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        protected override string BaseConnectionString => "Server=(local)\\SQL2016; UID=sa;PWD=Password12!; Persist Security Info = 'TRUE'";
    }
}
