using System;
using System.Data.OleDb;
using System.Linq;

namespace Kros.Data.MsAccess
{
    /// <summary>
    /// General extensions for Microsoft Access.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns Microsoft Access error code.
        /// </summary>
        /// <param name="ex">Exception, whose error code is checked.</param>
        /// <returns>Returns error code as an enumeration <see cref="MsAccess.MsAccessErrorCode" />. If the error code
        /// is unknown, or not defined, value <see cref="MsAccess.MsAccessErrorCode">MsAccess.MsAccessErrorCode</see> is returned.
        /// </returns>
        /// <remarks>
        /// Method checks the value of <see cref="OleDbError.SQLState">SQLState</see> of the first error in the list
        /// <see cref="OleDbException.Errors">OleDbException.Errors</see>.
        /// </remarks>
        public static MsAccessErrorCode MsAccessErrorCode(this OleDbException ex)
        {
            return MsAccessErrorCode(ex, out string sqlState);
        }

        /// <summary>
        /// Returns Microsoft Access error code.
        /// </summary>
        /// <param name="ex">Exception, whose error code is checked.</param>
        /// <param name="sqlState">Real value of <see cref="OleDbError.SQLState">SQLState</see>.</param>
        /// <returns>Returns error code as an enumeration <see cref="MsAccess.MsAccessErrorCode" />. If the error code
        /// is unknown, or not defined, value <see cref="MsAccess.MsAccessErrorCode">MsAccess.MsAccessErrorCode</see> is returned.
        /// </returns>
        /// <remarks>
        /// Method checks the value of <see cref="OleDbError.SQLState">SQLState</see> of the first error in the list
        /// <see cref="OleDbException.Errors">OleDbException.Errors</see>.
        /// </remarks>
        public static MsAccessErrorCode MsAccessErrorCode(this OleDbException ex, out string sqlState)
        {
            MsAccessErrorCode result = MsAccess.MsAccessErrorCode.Unknown;
            sqlState = string.Empty;

            if (ex.Errors.Count > 0)
            {
                sqlState = ex.Errors[0].SQLState;
                if ((!string.IsNullOrEmpty(sqlState)) && int.TryParse(sqlState, out int intState))
                {
                    int[] values = (int[])Enum.GetValues(typeof(MsAccessErrorCode));
                    if (values.Contains(intState))
                    {
                        result = (MsAccessErrorCode)intState;
                    }
                }
            }

            return result;
        }
    }
}
