using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kros.Data.Schema.SqlServer
{
    /// <summary>
    /// The implementation of <see cref="IDatabaseSchemaLoader{T}"/> for Microsoft SQL Server.
    /// </summary>
    public partial class SqlServerSchemaLoader
        : IDatabaseSchemaLoader<SqlConnection>
    {
        #region Helper mappings

        private static readonly Dictionary<SqlDbType, object> _defaultValueMapping = new Dictionary<SqlDbType, object>() {
            { SqlDbType.BigInt, ColumnSchema.DefaultValues.Int64 },
            { SqlDbType.Binary, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Bit, ColumnSchema.DefaultValues.Boolean },
            { SqlDbType.Char, ColumnSchema.DefaultValues.Text },
            { SqlDbType.DateTime, ColumnSchema.DefaultValues.DateTime },
            { SqlDbType.Decimal, ColumnSchema.DefaultValues.Decimal },
            { SqlDbType.Float, ColumnSchema.DefaultValues.Double },
            { SqlDbType.Image, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Int, ColumnSchema.DefaultValues.Int32 },
            { SqlDbType.Money, ColumnSchema.DefaultValues.Decimal },
            { SqlDbType.NChar, ColumnSchema.DefaultValues.Text },
            { SqlDbType.NText, ColumnSchema.DefaultValues.Text },
            { SqlDbType.NVarChar, ColumnSchema.DefaultValues.Text },
            { SqlDbType.Real, ColumnSchema.DefaultValues.Single },
            { SqlDbType.UniqueIdentifier, ColumnSchema.DefaultValues.Guid },
            { SqlDbType.SmallDateTime, ColumnSchema.DefaultValues.DateTime },
            { SqlDbType.SmallInt, ColumnSchema.DefaultValues.Int16 },
            { SqlDbType.SmallMoney, ColumnSchema.DefaultValues.Decimal },
            { SqlDbType.Text, ColumnSchema.DefaultValues.Text },
            { SqlDbType.Timestamp, ColumnSchema.DefaultValues.Null },
            { SqlDbType.TinyInt, ColumnSchema.DefaultValues.Byte },
            { SqlDbType.VarBinary, ColumnSchema.DefaultValues.Null },
            { SqlDbType.VarChar, ColumnSchema.DefaultValues.Text },
            { SqlDbType.Variant, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Xml, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Udt, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Structured, ColumnSchema.DefaultValues.Null },
            { SqlDbType.Date, ColumnSchema.DefaultValues.Date },
            { SqlDbType.Time, ColumnSchema.DefaultValues.Time },
            { SqlDbType.DateTime2, ColumnSchema.DefaultValues.DateTime },
            { SqlDbType.DateTimeOffset, ColumnSchema.DefaultValues.Null }
        };

        #endregion

        #region Events

        /// <summary>
        /// Event raised while parsing default value of a column. It is possible to use custom parsing logic in the event handler.
        /// </summary>
        /// <remarks>When custom logic for parsing column's default value is used, the parsed value is set in
        /// <see cref="SqlServerParseDefaultValueEventArgs.DefaultValue"/> property and
        /// <see cref="SqlServerParseDefaultValueEventArgs.Handled"/> flag must be set to <see langword="true"/>.</remarks>
        public event EventHandler<SqlServerParseDefaultValueEventArgs> ParseDefaultValue;

        /// <summary>
        /// Raises the <see cref="ParseDefaultValue"/> event with arguments <paramref name="e"/>.
        /// </summary>
        /// <param name="e">Arguments for the event.</param>
        protected virtual void OnParseDefaultValue(SqlServerParseDefaultValueEventArgs e)
        {
            ParseDefaultValue?.Invoke(this, e);
        }

        #endregion

        #region Schema loading

        /// <summary>
        /// Checks if it is poosible to load database schema for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns><see langword="false"/> if value of <paramref name="connection"/> is <see langword="null"/>,
        /// otherwise <see langword="true"/>.</returns>
        public bool SupportsConnectionType(SqlConnection connection)
        {
            return (connection != null);
        }

        /// <summary>
        /// Checks if it is poosible to load database schema for <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns><see langword="false"/> if value of <paramref name="connection"/> is not of <see cref="SqlConnection"/>
        /// type or is <see langword="null"/>, otherwise <see langword="true"/>.</returns>
        bool IDatabaseSchemaLoader.SupportsConnectionType(object connection)
        {
            return SupportsConnectionType(connection as SqlConnection);
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
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>Value of <paramref name="connection"/> is <see langword="null"/>.</item>
        /// <item>Initial catalog if <paramref name="connection"/> is <see langword="null"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item><paramref name="connection"/> is not of <see cref="SqlConnection"/> type.</item>
        /// <item>Initial catalog of <paramref name="connection"/> is an empty string, or string containing
        /// whitespace characters only.</item>
        /// </list>
        /// </exception>
        DatabaseSchema IDatabaseSchemaLoader.LoadSchema(object connection)
        {
            return LoadSchema(connection as SqlConnection);
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
        /// <item>Initial catalog if <paramref name="connection"/> is <see langword="null"/>.</item>
        /// <item>Value of <paramref name="tableName"/> is <see langword="null"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item><paramref name="connection"/> is not of <see cref="SqlConnection"/> type.</item>
        /// <item>Initial catalog of <paramref name="connection"/> is an empty string, or string containing
        /// whitespace characters only.</item>
        /// <item>Value of <paramref name="tableName"/> is an empty string, or string containing
        /// whitespace characters only.</item>
        /// </list>
        /// </exception>
        TableSchema IDatabaseSchemaLoader.LoadTableSchema(object connection, string tableName)
        {
            return LoadTableSchema(connection as SqlConnection, tableName);
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
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item>Value of <paramref name="connection"/> is <see langword="null"/>.</item>
        /// <item>Initial catalog if <paramref name="connection"/> is <see langword="null"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Initial catalog of <paramref name="connection"/> is an empty string, or string containing
        /// whitespace characters only.
        /// </exception>
        public DatabaseSchema LoadSchema(SqlConnection connection)
        {
            CheckConnection(connection);

            using (SqlConnection newConnection = (SqlConnection)(connection as ICloneable).Clone())
            {
                SqlConnection schemaConnection = newConnection;
                try
                {
                    newConnection.Open();
                }
                catch
                {
                    // Attempt to load schema using original connection in case new connection failed to open (is exclusive?).
                    // It would be great if we new how to check if a connection to SQL Server database is exclusive,
                    // so we would not need to use try-catch block.
                    newConnection.Dispose();
                    schemaConnection = connection;
                }
                using (ConnectionHelper.OpenConnection(schemaConnection))
                {
                    return LoadSchemaCore(schemaConnection);
                }
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
        /// <item>Initial catalog if <paramref name="connection"/> is <see langword="null"/>.</item>
        /// <item>Value of <paramref name="tableName"/> is <see langword="null"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <list type="bullet">
        /// <item>Initial catalog of <paramref name="connection"/> is an empty string, or string containing
        /// whitespace characters only.</item>
        /// <item>Value of <paramref name="tableName"/> is an empty string, or string containing
        /// whitespace characters only.</item>
        /// </list>
        /// </exception>
        public TableSchema LoadTableSchema(SqlConnection connection, string tableName)
        {
            CheckConnection(connection);
            Check.NotNullOrWhiteSpace(tableName, nameof(tableName));

            try
            {
                using (SqlConnection cn = (SqlConnection)(connection as ICloneable).Clone())
                {
                    cn.Open();
                    return LoadTableSchemaCore(cn, tableName);
                }
            }
            catch (Exception)
            {
                // Attempt to load schema using original connection in case it failed using new connection.
                // It would be great if we new how to check if a connection to SQL Server database is exclusive,
                // so we would not need to use try-catch block.
                return LoadTableSchemaCore(connection, tableName);
            }
        }

        private DatabaseSchema LoadSchemaCore(SqlConnection connection)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection.ConnectionString);
            DatabaseSchema database = new DatabaseSchema(builder.InitialCatalog);
            LoadTables(connection, database);
            LoadColumns(connection, database);
            LoadIndexes(connection, database);
            LoadForeignKeys(connection, database);

            return database;
        }

        #endregion

        #region Tables

        private TableSchema LoadTableSchemaCore(SqlConnection connection, string tableName)
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

        private void LoadTables(SqlConnection connection, DatabaseSchema database)
        {
            using (DataTable schemaData = GetSchemaTables(connection))
            {
                foreach (DataRow row in schemaData.Rows)
                {
                    database.Tables.Add((string)row[TablesSchemaNames.TableName]);
                }
            }
        }

        private void LoadColumns(SqlConnection connection, DatabaseSchema database)
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

        private SqlServerColumnSchema CreateColumnSchema(DataRow row, TableSchema table)
        {
            SqlServerColumnSchema column = new SqlServerColumnSchema((string)row[ColumnsSchemaNames.ColumnName])
            {
                AllowNull = ((string)row[ColumnsSchemaNames.IsNullable]).Equals("yes", StringComparison.OrdinalIgnoreCase),
                SqlDbType = GetSqlDbType(row)
            };
            column.DefaultValue = GetDefaultValue(row, column, table);
            if (!row.IsNull(ColumnsSchemaNames.CharacterMaximumLength))
            {
                column.Size = (int)row[ColumnsSchemaNames.CharacterMaximumLength];
            }
            if (!row.IsNull(ColumnsSchemaNames.DatetimePrecision))
            {
                System.Diagnostics.Debug.Assert(column.Size == 0,
                    "Setting DatetimePrecision but SqlServerColumnSchema.Size is already set.");
                column.Size = Convert.ToInt32(row[ColumnsSchemaNames.DatetimePrecision]);
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

        private SqlDbType GetSqlDbType(DataRow row)
        {
            SqlDbType sqlType = SqlDbType.Int;

            string dataType = (string)row[ColumnsSchemaNames.DataType];
            if (!Enum.TryParse(dataType, true, out sqlType))
            {
                if (dataType.Equals("numeric", StringComparison.OrdinalIgnoreCase))
                {
                    return SqlDbType.Decimal;
                }
                else
                {
                    return SqlDbType.Variant;
                }
            }

            return sqlType;
        }

        private object GetDefaultValue(DataRow row, SqlServerColumnSchema column, TableSchema table)
        {
            object defaultValue = null;
            string defaultValueString = null;

            if (row.IsNull(ColumnsSchemaNames.ColumnDefault))
            {
                defaultValue = column.AllowNull ? DBNull.Value : _defaultValueMapping[column.SqlDbType];
            }
            else
            {
                defaultValueString = GetDefaultValueString((string)row[ColumnsSchemaNames.ColumnDefault]);
                defaultValue = GetDefaultValueFromString(defaultValueString, column.SqlDbType);
            }

            SqlServerParseDefaultValueEventArgs e = new SqlServerParseDefaultValueEventArgs(
                table.Name, column.Name, column.SqlDbType, defaultValueString, defaultValue);
            OnParseDefaultValue(e);
            if (e.Handled)
            {
                defaultValue = e.DefaultValue;
            }

            if ((defaultValue == null) || (defaultValue == DBNull.Value))
            {
                return column.AllowNull ? DBNull.Value : _defaultValueMapping[column.SqlDbType];
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
            // Default values are in the database schema stored in a way, that the value is in parenthesis (sometimes doubled).
            // In the case of string columns, default value is also between apostrophes.
            // So default value 0 for numeric column is stored as "(0)" (or sometimes "((0))").
            // Default value "hello" for string column is stored as "('hello')" or "(N'hello')".
            rawDefaultValueString = rawDefaultValueString.Trim('(', ')');
            if (rawDefaultValueString.Length >= 2)
            {
                if ((rawDefaultValueString[0] == '\'') && (rawDefaultValueString[rawDefaultValueString.Length - 1] == '\''))
                {
                    return rawDefaultValueString.Substring(1, rawDefaultValueString.Length - 2);
                }
                if (((rawDefaultValueString[0] == 'N') || (rawDefaultValueString[0] == 'n')) &&
                    (rawDefaultValueString[1] == '\'') &&
                    (rawDefaultValueString[rawDefaultValueString.Length - 1] == '\''))
                {
                    return rawDefaultValueString.Substring(2, rawDefaultValueString.Length - 3);
                }
            }
            return rawDefaultValueString;
        }

        private object GetDefaultValueFromString(string defaultValueString, SqlDbType dataType)
        {
            object result = null;

            if ((dataType == SqlDbType.NText) ||
                (dataType == SqlDbType.NVarChar) ||
                (dataType == SqlDbType.Text) ||
                (dataType == SqlDbType.VarChar) ||
                (dataType == SqlDbType.NChar) ||
                (dataType == SqlDbType.Char))
            {
                result = defaultValueString;
            }
            else
            {
                result = GetParseFunction(dataType)?.Invoke(defaultValueString);
            }

            return result;
        }

        private DefaultValueParsers.ParseDefaultValueFunction GetParseFunction(SqlDbType dataType)
        {
            switch (dataType)
            {
                case SqlDbType.BigInt:
                    return DefaultValueParsers.ParseInt64;

                case SqlDbType.Int:
                    return DefaultValueParsers.ParseInt32;

                case SqlDbType.SmallInt:
                    return DefaultValueParsers.ParseInt16;

                case SqlDbType.TinyInt:
                    return DefaultValueParsers.ParseByte;

                case SqlDbType.Bit:
                    return DefaultValueParsers.ParseBool;

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                    return DefaultValueParsers.ParseDecimal;

                case SqlDbType.Float:
                    return DefaultValueParsers.ParseDouble;

                case SqlDbType.Real:
                    return DefaultValueParsers.ParseSingle;

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.SmallDateTime:
                    return DefaultValueParsers.ParseDateSql;

                case SqlDbType.DateTimeOffset:
                    return DefaultValueParsers.ParseDateTimeOffsetSql;

                case SqlDbType.UniqueIdentifier:
                    return DefaultValueParsers.ParseGuid;
            }

            return null;
        }

        #endregion

        #region Indexes

        private static class IndexesQueryNames
        {
            public const string TableName = "TableName";
            public const string IndexName = "IndexName";
            public const string IndexId = "IndexId";
            public const string IsUnique = "IsUnique";
            public const string IsPrimaryKey = "IsPrimaryKey";
            public const string IsDisabled = "IsDisabled";
            public const string TypDesc = "TypDesc";
            public const string ColumnName = "ColumnName";
            public const string IsDesc = "IsDesc";
            public const string ColumnId = "ColumnId";
            public const string ColumnOrdinal = "ColumnOrdinal";
        }

        private readonly string LoadIndexesQuery =
$@"SELECT
    tables.name AS {IndexesQueryNames.TableName},
    indexes.name AS {IndexesQueryNames.IndexName},
    indexes.index_id AS {IndexesQueryNames.IndexId},
    indexes.is_unique AS {IndexesQueryNames.IsUnique},
    indexes.is_primary_key AS {IndexesQueryNames.IsPrimaryKey},
    indexes.is_disabled AS {IndexesQueryNames.IsDisabled},
    indexes.type_desc AS {IndexesQueryNames.TypDesc},
    columns.name AS {IndexesQueryNames.ColumnName},
    index_columns.is_descending_key AS {IndexesQueryNames.IsDesc},
    index_columns.index_column_id AS {IndexesQueryNames.ColumnId},
    index_columns.key_ordinal AS {IndexesQueryNames.ColumnOrdinal}

FROM sys.indexes indexes

INNER JOIN sys.index_columns index_columns
    ON indexes.object_id = index_columns.object_id and indexes.index_id = index_columns.index_id

INNER JOIN sys.columns columns
    ON index_columns.object_id = columns.object_id and index_columns.column_id = columns.column_id

INNER JOIN sys.tables tables
    ON indexes.object_id = tables.object_id

ORDER BY tables.name, indexes.name, index_columns.key_ordinal
";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private void LoadIndexes(SqlConnection connection, DatabaseSchema database)
        {
            using (DataTable schemaData = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(LoadIndexesQuery, connection))
                {
                    adapter.Fill(schemaData);
                }
                foreach (TableSchema table in database.Tables)
                {
                    schemaData.DefaultView.RowFilter = $"{IndexesQueryNames.TableName} = '{table.Name}'";
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
                string indexName = (string)rowView.Row[IndexesQueryNames.IndexName];
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
            string indexName = (string)row[IndexesQueryNames.IndexName];
            bool clustered = ((string)row[IndexesQueryNames.TypDesc]).Equals("CLUSTERED", StringComparison.OrdinalIgnoreCase);
            if ((bool)row[IndexesQueryNames.IsPrimaryKey])
            {
                return table.SetPrimaryKey(indexName, clustered);
            }
            else
            {
                IndexType indexType = (bool)row[IndexesQueryNames.IsUnique] ? IndexType.UniqueKey : IndexType.Index;
                return table.Indexes.Add(indexName, indexType, clustered);
            }
        }

        private void AddColumnToIndex(IndexSchema index, DataRow row)
        {
            index.Columns.Add(
                (string)row[IndexesQueryNames.ColumnName],
                (bool)row[IndexesQueryNames.IsDesc] ? SortOrder.Descending : SortOrder.Ascending);
        }

        #endregion

        #region Foreign keys

        private static class ForeignKeyQueryNames
        {
            public const string ForeignKeyId = "ForeignKeyId";
            public const string ForeignKeyName = "ForeignKeyName";
            public const string ReferencedTableName = "ReferencedTableName";
            public const string ParentTableName = "ParentTableName";
            public const string DeleteRule = "DeleteRule";
            public const string UpdateRule = "UpdateRule";
        }

        private static class ForeignKeyColumnsQueryNames
        {
            public const string ForeignKeyId = "ForeignKeyId";
            public const string ParentColumnName = "ParentColumnName";
            public const string ReferencedColumnName = "ReferencedColumnName";
        }

        private readonly string LoadForeignKeysQuery =
$@"SELECT
    [object_id] AS {ForeignKeyQueryNames.ForeignKeyId},
    [name] AS {ForeignKeyQueryNames.ForeignKeyName},
    OBJECT_NAME([referenced_object_id]) AS {ForeignKeyQueryNames.ReferencedTableName},
    OBJECT_NAME([parent_object_id]) AS {ForeignKeyQueryNames.ParentTableName},
    [delete_referential_action_desc] AS {ForeignKeyQueryNames.DeleteRule},
    [update_referential_action_desc] AS {ForeignKeyQueryNames.UpdateRule}

FROM sys.foreign_keys

WHERE [type_desc] = 'FOREIGN_KEY_CONSTRAINT'
";

        private readonly string LoadForeignKeyColumnsQuery =
$@"SELECT
    foreign_key_columns.constraint_object_id AS {ForeignKeyColumnsQueryNames.ForeignKeyId},
    ParentColumns.name AS {ForeignKeyColumnsQueryNames.ParentColumnName},
    ReferencedColumns.name AS {ForeignKeyColumnsQueryNames.ReferencedColumnName}

FROM sys.foreign_key_columns foreign_key_columns

INNER JOIN sys.columns ParentColumns ON
    foreign_key_columns.parent_object_id = ParentColumns.object_id AND
    foreign_key_columns.parent_column_id = ParentColumns.column_id

INNER JOIN sys.columns ReferencedColumns ON
    foreign_key_columns.referenced_object_id = ReferencedColumns.object_id AND
    foreign_key_columns.referenced_column_id = ReferencedColumns.column_id

WHERE foreign_key_columns.constraint_object_id IN (
    SELECT [object_id] FROM sys.foreign_keys WHERE [type_desc] = 'FOREIGN_KEY_CONSTRAINT'
)

ORDER BY foreign_key_columns.constraint_object_id
";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private void LoadForeignKeys(SqlConnection connection, DatabaseSchema database)
        {
            using (DataTable foreignKeysData = new DataTable("ForeignKeys"))
            using (DataTable foreignKeyColumnsData = new DataTable("ForeignKeys"))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(LoadForeignKeysQuery, connection))
                {
                    adapter.Fill(foreignKeysData);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(LoadForeignKeyColumnsQuery, connection))
                {
                    adapter.Fill(foreignKeyColumnsData);
                }
                LoadForeignKeysSchema(database, foreignKeysData, foreignKeyColumnsData);
            }
        }

        private void LoadForeignKeysSchema(DatabaseSchema database, DataTable foreignKeysData, DataTable foreignKeyColumnsData)
        {
            DataView columnsView = foreignKeyColumnsData.DefaultView;
            List<string> primaryKeyColumns = new List<string>();
            List<string> foreignKeyColumns = new List<string>();
            foreach (DataRow fkRow in foreignKeysData.Rows)
            {
                int foreignKeyId = (int)fkRow[ForeignKeyQueryNames.ForeignKeyId];
                columnsView.RowFilter = $"[{ForeignKeyColumnsQueryNames.ForeignKeyId}] = {foreignKeyId}";

                primaryKeyColumns.Clear();
                foreignKeyColumns.Clear();
                foreach (DataRowView fkColumnRow in columnsView)
                {
                    primaryKeyColumns.Add((string)fkColumnRow.Row[ForeignKeyColumnsQueryNames.ReferencedColumnName]);
                    foreignKeyColumns.Add((string)fkColumnRow.Row[ForeignKeyColumnsQueryNames.ParentColumnName]);
                }
                ForeignKeySchema foreignKey = CreateForeignKey(fkRow, primaryKeyColumns, foreignKeyColumns);
                database.Tables[(string)fkRow[ForeignKeyQueryNames.ParentTableName]].ForeignKeys.Add(foreignKey);
            }
        }

        private ForeignKeySchema CreateForeignKey(
            DataRow foreignKeyData,
            List<string> primaryKeyColumns,
            List<string> foreignKeyColumns)
        {
            ForeignKeySchema foreignKey = new ForeignKeySchema(
                (string)foreignKeyData[ForeignKeyQueryNames.ForeignKeyName],
                (string)foreignKeyData[ForeignKeyQueryNames.ReferencedTableName],
                primaryKeyColumns,
                (string)foreignKeyData[ForeignKeyQueryNames.ParentTableName],
                foreignKeyColumns);
            foreignKey.DeleteRule = GetForeignKeyRule((string)foreignKeyData[ForeignKeyQueryNames.DeleteRule]);
            foreignKey.UpdateRule = GetForeignKeyRule((string)foreignKeyData[ForeignKeyQueryNames.UpdateRule]);

            return foreignKey;
        }

        private ForeignKeyRule GetForeignKeyRule(string ruleDesc)
        {
            if (ruleDesc.Equals("CASCADE", StringComparison.OrdinalIgnoreCase))
            {
                return ForeignKeyRule.Cascade;
            }
            else if (ruleDesc.Equals("SET_NULL", StringComparison.OrdinalIgnoreCase))
            {
                return ForeignKeyRule.SetNull;
            }
            else if (ruleDesc.Equals("SET_DEFAULT", StringComparison.OrdinalIgnoreCase))
            {
                return ForeignKeyRule.SetDefault;
            }
            else
            {
                return ForeignKeyRule.NoAction;
            }
        }

        #endregion

        #region Helpers

        private void CheckConnection(object connection)
        {
            Check.NotNull(connection, nameof(connection));
            if (!(this as IDatabaseSchemaLoader).SupportsConnectionType(connection))
            {
                throw new ArgumentException(Resources.SqlServerUnsupportedConnectionType, nameof(connection));
            }
            SqlConnectionStringBuilder cnBuilder = new SqlConnectionStringBuilder((connection as SqlConnection).ConnectionString);
            Check.NotNullOrWhiteSpace(
                cnBuilder.InitialCatalog, nameof(connection), Resources.SqlServerNoInitialCatalog);
        }

        private DataTable GetSchemaTables(SqlConnection connection)
        {
            return GetSchemaTables(connection, null);
        }

        private DataTable GetSchemaTables(SqlConnection connection, string tableName)
        {
            return connection.GetSchema(SchemaNames.Tables, new string[] { null, null, tableName, null });
        }

        private DataTable GetSchemaColumns(SqlConnection connection)
        {
            return GetSchemaColumns(connection, null);
        }

        private DataTable GetSchemaColumns(SqlConnection connection, string tableName)
        {
            DataTable schemaData = connection.GetSchema(SchemaNames.Columns, new string[] { null, null, tableName, null });
            schemaData.DefaultView.Sort =
                $"{ColumnsSchemaNames.TableSchema}, {ColumnsSchemaNames.TableName}, {ColumnsSchemaNames.OrdinalPosition}";
            return schemaData;
        }

        #endregion
    }
}
