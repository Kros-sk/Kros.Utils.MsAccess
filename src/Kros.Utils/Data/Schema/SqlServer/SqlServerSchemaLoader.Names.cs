namespace Kros.Data.Schema.SqlServer
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class SqlServerSchemaLoader
    {
        public static class SchemaNames
        {
            public const string MetaDataCollections = "MetaDataCollections";
            public const string Restrictions = "Restrictions";
            public const string DataSourceInformation = "DataSourceInformation";
            public const string Databases = "Databases";
            public const string Tables = "Tables";
            public const string Columns = "Columns";
            public const string AllColumns = "AllColumns";
            public const string ColumnSetColumns = "ColumnSetColumns";
            public const string Indexes = "Indexes";
            public const string IndexColumns = "IndexColumns";
            public const string ForeignKeys = "ForeignKeys";
            public const string Views = "Views";
            public const string ViewColumns = "ViewColumns";
            public const string Procedures = "Procedures";
            public const string ProcedureParameters = "ProcedureParameters";
            public const string DataTypes = "DataTypes";
            public const string UserDefinedTypes = "UserDefinedTypes";
            public const string Users = "Users";
            public const string StructuredTypeMembers = "StructuredTypeMembers";
            public const string ReservedWords = "ReservedWords";
        }

        public static class DatabasesSchemaNames
        {
            public const string DatabaseName = "DATABASE_NAME";
            public const string DbId = "DBID";
            public const string CreateDate = "CREATE_DATE";
        }

        public static class TablesSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string TableType = "TABLE_TYPE";
        }

        public static class ColumnsSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ColumnName = "COLUMN_NAME";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string ColumnDefault = "COLUMN_DEFAULT";
            public const string IsNullable = "IS_NULLABLE";
            public const string DataType = "DATA_TYPE";
            public const string CharacterMaximumLength = "CHARACTER_MAXIMUM_LENGTH";
            public const string CharacterOctetLength = "CHARACTER_OCTET_LENGTH";
            public const string NumericPrecision = "NUMERIC_PRECISION";
            public const string NumericPrecision_radix = "NUMERIC_PRECISION_RADIX";
            public const string NumericScale = "NUMERIC_SCALE";
            public const string DatetimePrecision = "DATETIME_PRECISION";
            public const string CharacterSetCatalog = "CHARACTER_SET_CATALOG";
            public const string CharacterSetSchema = "CHARACTER_SET_SCHEMA";
            public const string CharacterSetName = "CHARACTER_SET_NAME";
            public const string CollationCatalog = "COLLATION_CATALOG";
            public const string IsFilestream = "IS_FILESTREAM";
            public const string IsSparse = "IS_SPARSE";
            public const string IsColumnSet = "IS_COLUMN_SET";
        }

        public static class ColumnSetColumnsSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ColumnName = "COLUMN_NAME";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string ColumnDefault = "COLUMN_DEFAULT";
            public const string IsNullable = "IS_NULLABLE";
            public const string DataType = "DATA_TYPE";
            public const string CharacterMaximumLength = "CHARACTER_MAXIMUM_LENGTH";
            public const string CharacterOctetLength = "CHARACTER_OCTET_LENGTH";
            public const string NumericPrecision = "NUMERIC_PRECISION";
            public const string NumericPrecisionRadix = "NUMERIC_PRECISION_RADIX";
            public const string NumericScale = "NUMERIC_SCALE";
            public const string DatetimePrecision = "DATETIME_PRECISION";
            public const string CharacterSetCataloG = "CHARACTER_SET_CATALOG";
            public const string CharacterSetSchema = "CHARACTER_SET_SCHEMA";
            public const string CharacterSetName = "CHARACTER_SET_NAME";
            public const string CollationCatalog = "COLLATION_CATALOG";
            public const string IsFilestream = "IS_FILESTREAM";
            public const string IsSparse = "IS_SPARSE";
            public const string IsColumnSet = "IS_COLUMN_SET";
        }

        public static class IndexesSchemaNames
        {
            public const string ConstraintCatalog = "CONSTRAINT_CATALOG";
            public const string ConstraintSchema = "CONSTRAINT_SCHEMA";
            public const string ConstraintName = "CONSTRAINT_NAME";
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string TypeDesc = "TYPE_DESC";
        }

        public static class IndexColumnsSchemaNames
        {
            public const string ConstraintCatalog = "CONSTRAINT_CATALOG";
            public const string ConstraintSchema = "CONSTRAINT_SCHEMA";
            public const string ConstraintName = "CONSTRAINT_NAME";
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ColumnName = "COLUMN_NAME";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string KeyType = "KEYTYPE";
        }

        public static class ForeignKeysSchemaNames
        {
            public const string ConstraintCatalog = "CONSTRAINT_CATALOG";
            public const string ConstraintSchema = "CONSTRAINT_SCHEMA";
            public const string ConstraintName = "CONSTRAINT_NAME";
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ConstraintType = "CONSTRAINT_TYPE";
            public const string IsDeferrable = "IS_DEFERRABLE";
            public const string InitiallyDeferred = "INITIALLY_DEFERRED";
        }

        public static class ViewsSchemaNames
        {
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string CheckOption = "CHECK_OPTION";
            public const string IsUpdatable = "IS_UPDATABLE";
        }

        public static class ViewColumnsSchemaNames
        {
            public const string ViewCatalog = "VIEW_CATALOG";
            public const string ViewSchema = "VIEW_SCHEMA";
            public const string ViewName = "VIEW_NAME";
            public const string TableCatalog = "TABLE_CATALOG";
            public const string TableSchema = "TABLE_SCHEMA";
            public const string TableName = "TABLE_NAME";
            public const string ColumnName = "COLUMN_NAME";
        }

        public static class ProceduresSchemaNames
        {
            public const string SpecificCatalog = "SPECIFIC_CATALOG";
            public const string SpecificSchema = "SPECIFIC_SCHEMA";
            public const string SpecificName = "SPECIFIC_NAME";
            public const string RoutineCatalog = "ROUTINE_CATALOG";
            public const string RoutineSchema = "ROUTINE_SCHEMA";
            public const string RoutineName = "ROUTINE_NAME";
            public const string RoutineType = "ROUTINE_TYPE";
            public const string Created = "CREATED";
            public const string LastAltered = "LAST_ALTERED";
        }

        public static class ProcedureParametersSchemaNames
        {
            public const string SpecificCatalog = "SPECIFIC_CATALOG";
            public const string SpecificSchema = "SPECIFIC_SCHEMA";
            public const string SpecificName = "SPECIFIC_NAME";
            public const string OrdinalPosition = "ORDINAL_POSITION";
            public const string ParameterMode = "PARAMETER_MODE";
            public const string IsResult = "IS_RESULT";
            public const string AsLocator = "AS_LOCATOR";
            public const string ParameterName = "PARAMETER_NAME";
            public const string DataType = "DATA_TYPE";
            public const string CharacterMaximumLength = "CHARACTER_MAXIMUM_LENGTH";
            public const string CharacterOctetLength = "CHARACTER_OCTET_LENGTH";
            public const string CollationCatalog = "COLLATION_CATALOG";
            public const string CollationSchema = "COLLATION_SCHEMA";
            public const string CollationName = "COLLATION_NAME";
            public const string CharacterSetCatalog = "CHARACTER_SET_CATALOG";
            public const string CharacterSetSchema = "CHARACTER_SET_SCHEMA";
            public const string CharacterSetName = "CHARACTER_SET_NAME";
            public const string NumericPrecision = "NUMERIC_PRECISION";
            public const string NumericPrecisionRadix = "NUMERIC_PRECISION_RADIX";
            public const string NumericScale = "NUMERIC_SCALE";
            public const string DatetimePrecision = "DATETIME_PRECISION";
            public const string IntervalType = "INTERVAL_TYPE";
            public const string IntervalPrecision = "INTERVAL_PRECISION";
        }

        public static class UserDefinedTypesSchemaNames
        {
            public const string AssemblyName = "ASSEMBLY_NAME";
            public const string UdtName = "UDT_NAME";
            public const string VersionMajor = "VERSION_MAJOR";
            public const string VersionMinor = "VERSION_MINOR";
            public const string VersionBuild = "VERSION_BUILD";
            public const string VersionRevision = "VERSION_REVISION";
            public const string CultureInfo = "CULTURE_INFO";
            public const string PublicKey = "PUBLIC_KEY";
            public const string IsFixedLength = "IS_FIXED_LENGTH";
            public const string MaxLength = "MAX_LENGTH";
            public const string PermissionSetDesc = "PERMISSION_SET_DESC";
            public const string CreateDate = "CREATE_DATE";
        }

        public static class UsersSchemaNames
        {
            public const string Uid = "UID";
            public const string Name = "NAME";
            public const string CreateDate = "CREATEDATE";
            public const string UpdateDate = "UPDATEDATE";
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
