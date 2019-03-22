namespace Kros.Data.Schema.MsAccess
{
    public partial class MsAccessSchemaLoader
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static class TableTypes
        {
            public const string AccessTable = "ACCESS TABLE";
            public const string SystemTable = "SYSTEM TABLE";
            public const string Table = "TABLE";
        }

        public static class SchemaNames
        {
            public const string MetaDataCollections = "MetaDataCollections";
            public const string DataSourceInformation = "DataSourceInformation";
            public const string DataTypes = "DataTypes";
            public const string Restrictions = "Restrictions";
            public const string ReservedWords = "ReservedWords";
            public const string Columns = "Columns";
            public const string Indexes = "Indexes";
            public const string Procedures = "Procedures";
            public const string Tables = "Tables";
            public const string Views = "Views";
        }

        public static class TablesSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string TableType = "TABLE_TYPE";
            public const string TableGuid = "TABLE_GUID";
            public const string Description = "DESCRIPTION";
            public const string TablePropid = "TABLE_PROPID";
            public const string DateCreated = "DATE_CREATED";
            public const string DateModified = "DATE_MODIFIED";
        }

        public static class ColumnsSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ColumnName = "COLUMN_NAME";
            public const string ColumnGuid = "COLUMN_GUID";
            public const string ColumnPropid = "COLUMN_PROPID";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string ColumnHasdefault = "COLUMN_HASDEFAULT";
            public const string ColumnDefault = "COLUMN_DEFAULT";
            public const string ColumnFlags = "COLUMN_FLAGS";
            public const string IsNullable = "IS_NULLABLE";
            public const string DataType = "DATA_TYPE";
            public const string TypeGuid = "TYPE_GUID";
            public const string CharacterMaximumLength = "CHARACTER_MAXIMUM_LENGTH";
            public const string CharacterOctetLength = "CHARACTER_OCTET_LENGTH";
            public const string NumericPrecision = "NUMERIC_PRECISION";
            public const string NumericScale = "NUMERIC_SCALE";
            public const string DatetimePrecision = "DATETIME_PRECISION";
            public const string CharacterSet_catalog = "CHARACTER_SET_CATALOG";
            public const string CharacterSet_schema = "CHARACTER_SET_SCHEMA";
            public const string CharacterSet_name = "CHARACTER_SET_NAME";
            public const string CollationCatalog = "COLLATION_CATALOG";
            public const string CollationSchema = "COLLATION_SCHEMA";
            public const string CollationName = "COLLATION_NAME";
            public const string DomainCatalog = "DOMAIN_CATALOG";
            public const string DomainSchema = "DOMAIN_SCHEMA";
            public const string DomainName = "DOMAIN_NAME";
            public const string Description = "DESCRIPTION";
        }

        public static class IndexesSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string IndexCatalog = "INDEX_CATALOG";
            public const string IndexSchema = "INDEX_SCHEMA";
            public const string IndexName = "INDEX_NAME";
            public const string PrimaryKey = "PRIMARY_KEY";
            public const string Unique = "UNIQUE";
            public const string Clustered = "CLUSTERED";
            public const string Type = "TYPE";
            public const string FillFactor = "FILL_FACTOR";
            public const string InitialSize = "INITIAL_SIZE";
            public const string Nulls = "NULLS";
            public const string SortBookmarks = "SORT_BOOKMARKS";
            public const string AutoUpdate = "AUTO_UPDATE";
            public const string NullCollation = "NULL_COLLATION";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string ColumnName = "COLUMN_NAME";
            public const string ColumnGuid = "COLUMN_GUID";
            public const string ColumnPropid = "COLUMN_PROPID";
            public const string Collation = "COLLATION";
            public const string Cardinality = "CARDINALITY";
            public const string Pages = "PAGES";
            public const string FilterCondition = "FILTER_CONDITION";
            public const string Integrated = "INTEGRATED";
        }

        public static class ViewsSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ViewDefinition = "VIEW_DEFINITION";
            public const string CheckOption = "CHECK_OPTION";
            public const string IsUpdatable = "IS_UPDATABLE";
            public const string Description = "DESCRIPTION";
            public const string DateCreated = "DATE_CREATED";
            public const string DateModified = "DATE_MODIFIED";
        }

        public static class ProceduresSchemaNames
        {
            public const string ProcedureCatalog = "PROCEDURE_CATALOG";
            public const string ProcedureSchema = "PROCEDURE_SCHEMA";
            public const string ProcedureName = "PROCEDURE_NAME";
            public const string ProcedureType = "PROCEDURE_TYPE";
            public const string ProcedureDefinition = "PROCEDURE_DEFINITION";
            public const string Description = "DESCRIPTION";
            public const string DateCreated = "DATE_CREATED";
            public const string DateModified = "DATE_MODIFIED";
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
