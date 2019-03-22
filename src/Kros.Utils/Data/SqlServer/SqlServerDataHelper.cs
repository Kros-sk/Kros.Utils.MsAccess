namespace Kros.Data.SqlServer
{
    /// <summary>
    /// General helpers for Microsoft SQL Server.
    /// </summary>
    public static class SqlServerDataHelper
    {
        /// <summary>
        /// Identification of Microsoft SQL Server classes (used for example in <see cref="SqlServerIdGeneratorFactory"/>,
        /// <see cref="BulkActions.SqlServer.SqlServerBulkActionFactory"/>).
        /// </summary>
        public const string ClientId = "System.Data.SqlClient";
    }
}
