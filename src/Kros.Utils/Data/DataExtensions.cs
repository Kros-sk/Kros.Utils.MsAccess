using System;
using System.Data;
﻿using Kros.Data.SqlServer;
using System.Data.SqlClient;
using System.Linq;

namespace Kros.Data
{
    /// <summary>
    /// Extensions for various data classes.
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Checks if database connection is opened.
        /// </summary>
        /// <param name="cn">Database connection.</param>
        /// <returns><see langword="true"/>, if database connection is opened, <see langword="false"/> otherwise.</returns>
        public static bool IsOpened(this IDbConnection cn)
        {
            return cn.State.HasFlag(ConnectionState.Open);
        }

        /// <summary>
        /// Returns error code for Microsoft SQL Server.
        /// </summary>
        /// <param name="ex">Exception of which error code is tested.</param>
        /// <returns>Error code as a value of enumeration <see cref="SqlServerErrorCode"/>. If error code is not defined in
        /// the enum, value <see cref="SqlServerErrorCode.Unknown">SqlServerErrorCode.Unknown</see> is returned.</returns>
        /// <remarks>
        /// Method checks value of <see cref="SqlError.Number">Number</see> property of the first error in the
        /// <see cref="SqlException.Errors">SqlException.Errors</see> list.
        /// </remarks>
        public static SqlServerErrorCode SqlServerErrorCode(this SqlException ex)
        {
            if (ex.Errors.Count > 0)
            {
                int[] values = (int[])Enum.GetValues(typeof(SqlServerErrorCode));
                if (values.Contains(ex.Errors[0].Number))
                {
                    return (SqlServerErrorCode)ex.Errors[0].Number;
                }
            }

            return SqlServer.SqlServerErrorCode.Unknown;
        }
    }
}
