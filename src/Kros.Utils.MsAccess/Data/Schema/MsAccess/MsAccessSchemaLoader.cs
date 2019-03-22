using Kros.Data.MsAccess;
using Kros.MsAccess.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Kros.Data.Schema.MsAccess
{
    /// <summary>
    /// The implementation of <see cref="IDatabaseSchemaLoader{T}"/> for Microsoft Access.
    /// </summary>
    public partial class MsAccessSchemaLoader
        : IDatabaseSchemaLoader<OleDbConnection>
    {
        #region Helper mappings

        private static readonly Dictionary<OleDbType, object> _defaultValueMapping = new Dictionary<OleDbType, object>() {
            { OleDbType.BigInt, ColumnSchema.DefaultValues.Int64 },
            { OleDbType.Binary, ColumnSchema.DefaultValues.Null },
            { OleDbType.Boolean, ColumnSchema.DefaultValues.Boolean },
            { OleDbType.BSTR, ColumnSchema.DefaultValues.Text },
            { OleDbType.Currency, ColumnSchema.DefaultValues.Decimal },
            { OleDbType.Date, ColumnSchema.DefaultValues.DateTime },
            { OleDbType.DBDate, ColumnSchema.DefaultValues.Date },
            { OleDbType.DBTime, ColumnSchema.DefaultValues.Time },
            { OleDbType.DBTimeStamp, ColumnSchema.DefaultValues.DateTime },
            { OleDbType.Decimal, ColumnSchema.DefaultValues.Decimal },
            { OleDbType.Double, ColumnSchema.DefaultValues.Double },
            { OleDbType.Empty, ColumnSchema.DefaultValues.Null },
            { OleDbType.Error, ColumnSchema.DefaultValues.Int32 },
            { OleDbType.Filetime, ColumnSchema.DefaultValues.DateTime },
            { OleDbType.Guid, ColumnSchema.DefaultValues.Guid },
            { OleDbType.Char, ColumnSchema.DefaultValues.Text },
            { OleDbType.IDispatch, ColumnSchema.DefaultValues.Null },
            { OleDbType.Integer, ColumnSchema.DefaultValues.Int32 },
            { OleDbType.IUnknown, ColumnSchema.DefaultValues.Null },
            { OleDbType.LongVarBinary, ColumnSchema.DefaultValues.Null },
            { OleDbType.LongVarChar, ColumnSchema.DefaultValues.Text },
            { OleDbType.LongVarWChar, ColumnSchema.DefaultValues.Text },
            { OleDbType.Numeric, ColumnSchema.DefaultValues.Decimal },
            { OleDbType.PropVariant, ColumnSchema.DefaultValues.Null },
            { OleDbType.Single, ColumnSchema.DefaultValues.Single },
            { OleDbType.SmallInt, ColumnSchema.DefaultValues.Int16 },
            { OleDbType.TinyInt, ColumnSchema.DefaultValues.SByte },
            { OleDbType.UnsignedBigInt, ColumnSchema.DefaultValues.UInt64 },
            { OleDbType.UnsignedInt, ColumnSchema.DefaultValues.UInt32 },
            { OleDbType.UnsignedSmallInt, ColumnSchema.DefaultValues.UInt16 },
            { OleDbType.UnsignedTinyInt, ColumnSchema.DefaultValues.Byte },
            { OleDbType.VarBinary, ColumnSchema.DefaultValues.Null },
            { OleDbType.VarChar, ColumnSchema.DefaultValues.Text },
            { OleDbType.Variant, ColumnSchema.DefaultValues.Null },
            { OleDbType.VarNumeric, ColumnSchema.DefaultValues.Int32 },
            { OleDbType.VarWChar, ColumnSchema.DefaultValues.Text },
            { OleDbType.WChar, ColumnSchema.DefaultValues.Text }
        };

        #endregion

        #region Events

        /// <summary>
        /// Event raised while parsing default value of a column. It is possible to use custom parsing logic in the event handler.
        /// </summary>
        /// <remarks>When custom logic for parsing column's default value is used, the parsed value is set in
        /// <see cref="MsAccessParseDefaultValueEventArgs.DefaultValue"/> property and
        /// <see cref="MsAccessParseDefaultValueEventArgs.Handled"/> flag must be set to <see langword="true"/>.</remarks>
        public event EventHandler<MsAccessParseDefaultValueEventArgs> ParseDefaultValue;

        /// <summary>
        /// Raises the <see cref="ParseDefaultValue"/> event with arguments <paramref name="e"/>.
        /// </summary>
        /// <param name="e">Arguments for the event.</param>
        protected virtual void OnParseDefaultValue(MsAccessParseDefaultValueEventArgs e)
        {
            ParseDefaultValue?.Invoke(this, e);
        }

        #endregion

        #region Schema loading

        /// <inheritdoc cref="SupportsConnectionType(OleDbConnection)"/>
        bool IDatabaseSchemaLoader.SupportsConnectionType(object connection)
        {
            return SupportsConnectionType(connection as OleDbConnection);
        }

        /// <summary>
        /// Loads database schema for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Database schema.</returns>
        /// <remarks>By default, schema is loaded using new connection to the database, based on <paramref name="connection"/>.
        /// But if <paramref name="connection"/> is an exclusive connection, it is used directly.</remarks>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="connection"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The connection <paramref name="connection"/> is not a connection to Microsoft Access database.
        /// </exception>
        DatabaseSchema IDatabaseSchemaLoader.LoadSchema(object connection)
        {
            CheckConnection(connection);
            return LoadSchema(connection as OleDbConnection);
        }

        /// <summary>
        /// Loads table schema for table <paramref name="tableName"/> in database <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name.</param>
        /// <returns>Table schema, or value <see langword="null"/> if specified table does not exist.</returns>
        /// <remarks>By default, schema is loaded using new connection to the database, based on <paramref name="connection"/>.
        /// But if <paramref name="connection"/> is an exclusive connection, it is used directly.</remarks>
        /// <exception cref="ArgumentNullException">The value of <paramref name="connection"/> or <paramref name="tableName"/>
        /// is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <list>
        /// <item>The connection <paramref name="connection"/> is not a connection to Microsoft Access database.</item>
        /// <item>The value of <paramref name="tableName"/> is empty string, or string containing whitespace characters only.
        /// </item>
        /// </list>
        /// </exception>
        TableSchema IDatabaseSchemaLoader.LoadTableSchema(object connection, string tableName)
        {
            CheckConnection(connection);
            return LoadTableSchema(connection as OleDbConnection, tableName);
        }

        /// <summary>
        /// Checks if it is poosible to load database schema for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns><see langword="false"/> if value of <paramref name="connection"/> is <see langword="null"/>, or it is not
        /// a connection to Microsoft Access database. Otherwise <see langword="true"/>.</returns>
        public bool SupportsConnectionType(OleDbConnection connection)
        {
            return (connection != null) && MsAccessDataHelper.IsMsAccessConnection(connection);
        }

        /// <summary>
        /// Loads database schema for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Database schema.</returns>
        /// <remarks>
        /// Loading schema creates a new connection to database based on <paramref name="connection"/>. If loading with
        /// new connection fails (for example input connection is exclusive), schema is loaded using input
        /// <paramref name="connection"/> directly.</remarks>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> id <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The connection <paramref name="connection"/> is not a connection
        /// to Microsoft Access database.</exception>
        public DatabaseSchema LoadSchema(OleDbConnection connection)
        {
            CheckConnection(connection);

            if (MsAccessDataHelper.IsExclusiveMsAccessConnection(connection.ConnectionString))
            {
                return LoadSchemaCore(connection);
            }

            using (OleDbConnection cn = (OleDbConnection)(connection as ICloneable).Clone())
            {
                cn.Open();
                return LoadSchemaCore(cn);
            }
        }

        /// <summary>
        /// Loads table schema for table <paramref name="tableName"/> in database <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name.</param>
        /// <returns>Table schema, or value <see langword="null"/> if specified table does not exist.</returns>
        /// <remarks>
        /// Loading schema creates a new connection to database based on <paramref name="connection"/>. If loading with
        /// new connection fails (for example input connection is exclusive), schema is loaded using input
        /// <paramref name="connection"/> directly.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>Value of <paramref name="connection"/> is <see langword="null"/>.</item>
        /// <item>Value of <paramref name="tableName"/> is <see langword="null"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Value of <paramref name="tableName"/> is an empty string, or string containing whitespace characters only.
        /// </exception>
        public TableSchema LoadTableSchema(OleDbConnection connection, string tableName)
        {
            CheckConnection(connection);
            Check.NotNullOrWhiteSpace(tableName, nameof(tableName));

            if (MsAccessDataHelper.IsExclusiveMsAccessConnection(connection.ConnectionString))
            {
                return LoadTableSchemaCore(connection, tableName);
            }

            using (OleDbConnection cn = (OleDbConnection)(connection as ICloneable).Clone())
            {
                cn.Open();
                return LoadTableSchemaCore(cn, tableName);
            }
        }

        private DatabaseSchema LoadSchemaCore(OleDbConnection connection)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connection.ConnectionString);
            DatabaseSchema database = new DatabaseSchema(builder.DataSource);
            LoadTables(connection, database);
            LoadColumns(connection, database);
            LoadIndexes(connection, database);

            return database;
        }

        #endregion

        #region Tables

        private TableSchema LoadTableSchemaCore(OleDbConnection connection, string tableName)
        {
            TableSchema table = null;

            using (DataTable schemaData = GetSchemaTables(connection, tableName))
            {
                if (schemaData.Rows.Count == 1)
                {
                    table = new TableSchema(tableName);
                    using (DataTable columnsSchemaData = GetSchemaColumns(connection, tableName))
                    {
                        LoadColumns(table, columnsSchemaData);
                    }
                }
            }
            return table;
        }

        private void LoadTables(OleDbConnection connection, DatabaseSchema database)
        {
            using (DataTable schemaData = GetSchemaTables(connection))
            {
                foreach (DataRow row in schemaData.Rows)
                {
                    database.Tables.Add(row.Field<string>(TablesSchemaNames.TableName));
                }
            }
        }

        private void LoadColumns(OleDbConnection connection, DatabaseSchema database)
        {
            using (DataTable schemaData = GetSchemaColumns(connection))
            {
                foreach (TableSchema table in database.Tables)
                {
                    LoadColumns(table, schemaData);
                }
            }
        }

        private void LoadColumns(TableSchema table, DataTable columnsSchemaData)
        {
            columnsSchemaData.DefaultView.RowFilter = $"{ColumnsSchemaNames.TableName} = '{table.Name}'";
            foreach (DataRowView rowView in columnsSchemaData.DefaultView)
            {
                table.Columns.Add(CreateColumnSchema(rowView.Row, table));
            }
        }

        private MsAccessColumnSchema CreateColumnSchema(DataRow row, TableSchema table)
        {
            MsAccessColumnSchema column = new MsAccessColumnSchema(row.Field<string>(ColumnsSchemaNames.ColumnName));
            column.AllowNull = row.Field<bool>(ColumnsSchemaNames.IsNullable);
            column.OleDbType = GetOleDbType(row);
            column.DefaultValue = GetDefaultValue(row, column, table);
            if (!row.IsNull(ColumnsSchemaNames.CharacterMaximumLength))
            {
                column.Size = (int)row.Field<long>(ColumnsSchemaNames.CharacterMaximumLength);
            }
            if (!row.IsNull(ColumnsSchemaNames.NumericPrecision))
            {
                column.Precision = Convert.ToByte(row[ColumnsSchemaNames.NumericPrecision]);
            }
            if (!row.IsNull(ColumnsSchemaNames.NumericScale))
            {
                column.Scale = Convert.ToByte(row[ColumnsSchemaNames.NumericScale]);
            }

            return column;
        }

        private OleDbType GetOleDbType(DataRow row)
        {
            return (OleDbType)(row.Field<int>(ColumnsSchemaNames.DataType));
        }

        private object GetDefaultValue(DataRow row, MsAccessColumnSchema column, TableSchema table)
        {
            object defaultValue = null;
            string defaultValueString = null;

            if (row.IsNull(ColumnsSchemaNames.ColumnDefault))
            {
                defaultValue = column.AllowNull ? DBNull.Value : _defaultValueMapping[column.OleDbType];
            }
            else
            {
                defaultValueString = GetDefaultValueString(row.Field<string>(ColumnsSchemaNames.ColumnDefault));
                defaultValue = GetDefaultValueFromString(defaultValueString, column.OleDbType);
            }

            MsAccessParseDefaultValueEventArgs e = new MsAccessParseDefaultValueEventArgs(
                table.Name, column.Name, column.OleDbType, defaultValueString, defaultValue);
            OnParseDefaultValue(e);
            if (e.Handled)
            {
                defaultValue = e.DefaultValue;
            }

            if ((defaultValue == null) || (defaultValue == DBNull.Value))
            {
                return column.AllowNull ? DBNull.Value : _defaultValueMapping[column.OleDbType];
            }
            return defaultValue;
        }

        /// <summary>
        /// Adjusts the string <paramref name="rawDefaultValueString"/> so column's default value can be obtained from it.
        /// </summary>
        /// <param name="rawDefaultValueString">Default value string as it is stored in database.</param>
        /// <returns>Adjusted string - trimmed of unneeded characters.</returns>
        protected virtual string GetDefaultValueString(string rawDefaultValueString)
        {
            // Default value are in database stored in a way, that the value is enclosed in apostrophes.
            // In case of string column, it is also in quotation marks.
            // To remove them (Trim) is needed to do twice.
            return rawDefaultValueString.Trim('\'').Trim('"');
        }

        private object GetDefaultValueFromString(string defaultValueString, OleDbType dataType)
        {
            object result = null;

            if ((dataType == OleDbType.VarChar) ||
                (dataType == OleDbType.LongVarChar) ||
                (dataType == OleDbType.VarWChar) ||
                (dataType == OleDbType.LongVarWChar) ||
                (dataType == OleDbType.Char) ||
                (dataType == OleDbType.WChar))
            {
                result = defaultValueString;
            }
            else
            {
                result = GetParseFunction(dataType)?.Invoke(defaultValueString);
            }

            return result;
        }

        private DefaultValueParsers.ParseDefaultValueFunction GetParseFunction(OleDbType dataType)
        {
            switch (dataType)
            {
                case OleDbType.BigInt:
                    return DefaultValueParsers.ParseInt64;

                case OleDbType.Integer:
                    return DefaultValueParsers.ParseInt32;

                case OleDbType.SmallInt:
                    return DefaultValueParsers.ParseInt16;

                case OleDbType.TinyInt:
                    return DefaultValueParsers.ParseSByte;

                case OleDbType.Double:
                case OleDbType.Numeric:
                    return DefaultValueParsers.ParseDouble;

                case OleDbType.Single:
                    return DefaultValueParsers.ParseSingle;

                case OleDbType.Decimal:
                case OleDbType.Currency:
                    return DefaultValueParsers.ParseDecimal;

                case OleDbType.UnsignedBigInt:
                    return DefaultValueParsers.ParseUInt64;

                case OleDbType.UnsignedInt:
                    return DefaultValueParsers.ParseUInt32;

                case OleDbType.UnsignedSmallInt:
                    return DefaultValueParsers.ParseUInt16;

                case OleDbType.UnsignedTinyInt:
                    return DefaultValueParsers.ParseByte;

                case OleDbType.Guid:
                    return DefaultValueParsers.ParseGuid;

                case OleDbType.Boolean:
                    return DefaultValueParsers.ParseBool;

                case OleDbType.Date:
                    return DefaultValueParsers.ParseDate;
            }

            return null;
        }

        #endregion

        #region Indexes

        private void LoadIndexes(OleDbConnection connection, DatabaseSchema database)
        {
            using (DataTable schemaData = connection.GetSchema(SchemaNames.Indexes))
            {
                foreach (TableSchema table in database.Tables)
                {
                    schemaData.DefaultView.RowFilter = $"{IndexesSchemaNames.TableName} = '{table.Name}'";
                    schemaData.DefaultView.Sort = $"{IndexesSchemaNames.IndexName}, {IndexesSchemaNames.OrdinalPosition}";
                    if (schemaData.DefaultView.Count > 0)
                    {
                        LoadIndexesForTable(table, schemaData.DefaultView);
                    }
                }
            }
        }

        private void LoadIndexesForTable(TableSchema table, DataView schemaData)
        {
            string lastIndexName = string.Empty;
            IndexSchema index = null;

            foreach (DataRowView rowView in schemaData)
            {
                string indexName = rowView.Row.Field<string>(IndexesSchemaNames.IndexName);
                if (indexName != lastIndexName)
                {
                    lastIndexName = indexName;
                    index = CreateIndexSchema(table, rowView.Row);
                }
                AddColumnToIndex(index, rowView.Row);
            }
        }

        private IndexSchema CreateIndexSchema(TableSchema table, DataRow row)
        {
            string indexName = row.Field<string>(IndexesSchemaNames.IndexName);
            bool clustered = row.Field<bool>(IndexesSchemaNames.Clustered);
            if (row.Field<bool>(IndexesSchemaNames.PrimaryKey))
            {
                return table.SetPrimaryKey(indexName, clustered);
            }
            else
            {
                IndexType indexType = row.Field<bool>(IndexesSchemaNames.Unique) ? IndexType.UniqueKey : IndexType.Index;
                return table.Indexes.Add(indexName, indexType, clustered);
            }
        }

        private void AddColumnToIndex(IndexSchema index, DataRow row)
        {
            index.Columns.Add(
                row.Field<string>(IndexesSchemaNames.ColumnName),
                row.Field<short>(IndexesSchemaNames.Collation) == 2 ? SortOrder.Descending : SortOrder.Ascending);
        }

        #endregion

        #region Helpers

        private void CheckConnection(object connection)
        {
            Check.NotNull(connection, nameof(connection));
            if (!(this as IDatabaseSchemaLoader).SupportsConnectionType(connection))
            {
                throw new ArgumentException(Resources.UnsupportedConnectionType, nameof(connection));
            }
        }

        private DataTable GetSchemaTables(OleDbConnection connection)
        {
            return GetSchemaTables(connection, null);
        }

        private DataTable GetSchemaTables(OleDbConnection connection, string tableName)
        {
            return connection.GetSchema(SchemaNames.Tables, new string[] { null, null, tableName, TableTypes.Table });
        }

        private DataTable GetSchemaColumns(OleDbConnection connection)
        {
            return GetSchemaColumns(connection, null);
        }

        private DataTable GetSchemaColumns(OleDbConnection connection, string tableName)
        {
            DataTable schemaData = connection.GetSchema(SchemaNames.Columns, new string[] { null, null, tableName, null });
            schemaData.DefaultView.Sort =
                $"{ColumnsSchemaNames.TableSchema}, {ColumnsSchemaNames.TableName}, {ColumnsSchemaNames.OrdinalPosition}";
            return schemaData;
        }

        #endregion
    }
}
