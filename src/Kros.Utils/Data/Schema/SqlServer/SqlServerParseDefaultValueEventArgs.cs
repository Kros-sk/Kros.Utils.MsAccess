using System.Data;

namespace Kros.Data.Schema.SqlServer
{
    /// <summary>
    /// Arguments for the event <see cref="SqlServerSchemaLoader.ParseDefaultValue"/>.
    /// </summary>
    public class SqlServerParseDefaultValueEventArgs
        : System.EventArgs
    {
        /// <summary>
        /// Creates and initializes instance of arguments.
        /// </summary>
        /// <param name="tableName"><inheritdoc cref="TableName"/></param>
        /// <param name="columnName"><inheritdoc cref="ColumnName"/></param>
        /// <param name="sqlDbType"><inheritdoc cref="SqlDbType"/></param>
        /// <param name="defaultValueString"><inheritdoc cref="DefaultValueString"/></param>
        /// <param name="defaultValue">Value, which was obtained by default parser.</param>
        public SqlServerParseDefaultValueEventArgs(
            string tableName,
            string columnName,
            SqlDbType sqlDbType,
            string defaultValueString,
            object defaultValue)
        {
            TableName = tableName;
            ColumnName = columnName;
            SqlDbType = sqlDbType;
            DefaultValueString = defaultValueString;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Table which schema is being loaded.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Column name which default value is being parsed.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Data type of column which default value is being parsed.
        /// </summary>
        public SqlDbType SqlDbType { get; }

        /// <summary>
        /// Default value of a column. It is the string which is being parsed.
        /// </summary>
        public string DefaultValueString { get; }

        /// <summary>
        /// Value, which was obtained by default parser. Set this value, when using custom logic for parsing default value
        /// in event handler.
        /// </summary>
        public object DefaultValue { get; set; } = null;

        /// <summary>
        /// Flag indicating if default value was parsed using custom logic in event handler. Set this to <see langword="true"/>
        /// if you set your own default value in <see cref="DefaultValue"/>.
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}
