namespace Kros.Data.MsAccess
{
    /// <summary>
    /// Error codes for Microsoft Access database provider.
    /// </summary>
    /// <remarks>The error code is in the exception <see cref="System.Data.OleDb.OleDbException">OleDbException</see>
    /// in the <see cref="System.Data.OleDb.OleDbError.SQLState">SqlState</see> property. The list of all error codes
    /// is at <see href="https://msdn.microsoft.com/en-us/library/bb221208(v=office.12).aspx" />.</remarks>
    public enum MsAccessErrorCode
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The changes you requested to the table were not successful because they would create duplicate values
        /// in the index, primary key, or relationship. Change the data in the field or fields that contain
        /// duplicate data, remove the index, or redefine the index to permit duplicate entries and try again.
        /// </summary>
        KeyDuplicity = 3022,

        /// <summary>
        /// Could not use &lt;name&gt;; file already in use.
        /// </summary>
        CouldNotUseFileAlreadyInUse = 3045,

        /// <summary>
        /// Cannot open database 'name'. It may not be a database that your application recognizes,
        /// or the file may be corrupt.
        /// </summary>
        CannotOpenDatabaseFileIsInvalid = 3049,

        /// <summary>
        /// The Microsoft Jet database engine cannot open the file 'name'. It is already opened exclusively
        /// by another user, or you need permission to view its data.
        /// </summary>
        CannotOpenDatabaseIsLocked = 3051,

        /// <summary>
        /// The Microsoft Jet database engine cannot find the input table or query 'name'.
        /// Make sure it exists and that its name is spelled correctly.
        /// </summary>
        CannotFindTableOrQuery = 3078,

        /// <summary>
        /// You cannot add or change a record because a related record is required in table 'name'.
        /// </summary>
        RelationshipViolation = 3201,

        /// <summary>
        /// Could not update; currently locked.
        /// </summary>
        CouldNotUpdateCurrentlyLocked = 3218,

        /// <summary>
        /// You attempted to open a database that is already opened exclusively by user 'userName'
        /// on machine 'machineName'. Try again when the database is available.
        /// </summary>
        DatabaseAlreadyOpenedExclusively = 3356,

        /// <summary>
        /// Could not read the record; currently locked by another user.
        /// </summary>
        CouldNotReadRecordIsLocked = 3624
    }
}
