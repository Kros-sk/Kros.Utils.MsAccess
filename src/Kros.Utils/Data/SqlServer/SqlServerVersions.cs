using System;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// SQL Server versions. Version numbers are taken from this document:
    /// <see href="https://support.microsoft.com/en-us/help/321185/how-to-determine-the-version-edition-and-update-level-of-sql-server-an"/>.
    /// </summary>
    public static class SqlServerVersions
    {
        #region SQL Server 2005

        /// <summary>
        /// SQL Server 2005, version 9.00.1399.0.
        /// </summary>
        public static readonly Version Server2005 = new Version(9, 00, 1399);

        /// <summary>
        /// SQL Server 2005 SP1, version 9.00.2047.0.
        /// </summary>
        public static readonly Version Server2005SP1 = new Version(9, 00, 2047);

        /// <summary>
        /// SQL Server 2005 SP2, version 9.00.3042.0.
        /// </summary>
        public static readonly Version Server2005SP2 = new Version(9, 00, 3042);

        /// <summary>
        /// SQL Server 2005 SP3, version 9.00.4035.0.
        /// </summary>
        public static readonly Version Server2005SP3 = new Version(9, 00, 4035);

        /// <summary>
        /// SQL Server 2005 SP4, version 9.00.5000.0.
        /// </summary>
        public static readonly Version Server2005SP4 = new Version(9, 00, 5000);

        #endregion

        #region SQL Server 2008

        /// <summary>
        /// SQL Server 2008, version 10.00.1600.22.
        /// </summary>
        public static readonly Version Server2008 = new Version(10, 00, 1600, 22);

        /// <summary>
        /// SQL Server 2008 SP1, version 10.00.2531.00.
        /// </summary>
        public static readonly Version Server2008SP1 = new Version(10, 00, 2531, 00);

        /// <summary>
        /// SQL Server 2008 SP2, version 10.00.4000.00.
        /// </summary>
        public static readonly Version Server2008SP2 = new Version(10, 00, 4000, 00);

        /// <summary>
        /// SQL Server 2008 SP3, version 10.00.5500.00.
        /// </summary>
        public static readonly Version Server2008SP3 = new Version(10, 00, 5500, 00);

        #endregion

        #region SQL Server 2008 R2

        /// <summary>
        /// SQL Server 2008 R2, version 10.50.1600.1.
        /// </summary>
        public static readonly Version Server2008R2 = new Version(10, 50, 1600, 1);

        /// <summary>
        /// SQL Server 2008 R2 SP1, version 10.50.2500.0.
        /// </summary>
        public static readonly Version Server2008R2SP1 = new Version(10, 50, 2500, 0);

        /// <summary>
        /// SQL Server 2008 R2 SP2, version 10.50.4000.0.
        /// </summary>
        public static readonly Version Server2008R2SP2 = new Version(10, 50, 4000, 0);

        /// <summary>
        /// SQL Server 2008 R2 SP3, version 10.50.6000.34.
        /// </summary>
        public static readonly Version Server2008R2SP3 = new Version(10, 50, 6000, 34);

        #endregion

        #region SQL Server 2012

        /// <summary>
        /// SQL Server 2012, version 11.0.2100.60.
        /// </summary>
        public static readonly Version Server2012 = new Version(11, 0, 2100, 60);

        /// <summary>
        /// SQL Server 2012 SP1, version 11.0.3000.00.
        /// </summary>
        public static readonly Version Server2012SP1 = new Version(11, 0, 3000, 00);

        /// <summary>
        /// SQL Server 2012 SP2, version 11.0.5058.0.
        /// </summary>
        public static readonly Version Server2012SP2 = new Version(11, 0, 5058, 0);

        /// <summary>
        /// SQL Server 2012 SP3, version 11.0.6020.0.
        /// </summary>
        public static readonly Version Server2012SP3 = new Version(11, 0, 6020, 0);

        /// <summary>
        /// SQL Server 2012 SP4, version 11.0.7001.0.
        /// </summary>
        public static readonly Version Server2012SP4 = new Version(11, 0, 7001, 0);

        #endregion

        #region SQL Server 2014

        /// <summary>
        /// SQL Server 2014, version 12.0.2000.8.
        /// </summary>
        public static readonly Version Server2014 = new Version(12, 0, 2000, 8);

        /// <summary>
        /// SQL Server 2014 SP1, version 12.0.4100.1.
        /// </summary>
        public static readonly Version Server2014SP1 = new Version(12, 0, 4100, 1);

        /// <summary>
        /// SQL Server 2014 SP2, version 12.0.5000.0.
        /// </summary>
        public static readonly Version Server2014SP2 = new Version(12, 0, 5000, 0);

        #endregion

        #region SQL Server 2016

        /// <summary>
        /// SQL Server 2016, version 13.0.2149.0.
        /// </summary>
        public static readonly Version Server2016 = new Version(13, 0, 2149, 0);

        /// <summary>
        /// SQL Server 2016 SP1, version 13.0.4001.0.
        /// </summary>
        public static readonly Version Server2016SP1 = new Version(13, 0, 4001, 0);

        /// <summary>
        /// SQL Server 2016 SP2, version 13.0.5026.0.
        /// </summary>
        public static readonly Version Server2016SP2 = new Version(13, 0, 5026, 0);

        #endregion

        #region SQL Server 2017

        /// <summary>
        /// SQL Server 2017, version 14.0.3006.16.
        /// </summary>
        public static readonly Version Server2017 = new Version(14, 0, 3006, 16);

        #endregion
    }
}
