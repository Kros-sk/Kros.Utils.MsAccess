namespace Kros.Data.Schema
{
    /// <summary>
    /// Table index type.
    /// </summary>
    public enum IndexType
    {
        /// <summary>
        /// General index.
        /// </summary>
        Index = 0,

        /// <summary>
        /// Unique index.
        /// </summary>
        UniqueKey = 1,

        /// <summary>
        /// Primary key.
        /// </summary>
        PrimaryKey = 2
    }

    /// <summary>
    /// Sort order of an index column.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Ascending order.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Descending order.
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// Foreign key rule. Defines what to do with child records when the value of parent record changes or is deleted.
    /// </summary>
    public enum ForeignKeyRule
    {
        /// <summary>
        /// No action is taken. .
        /// </summary>
        NoAction = 0,

        /// <summary>
        /// Corresponding rows are updated in the referencing table when that row is updated in the parent table.
        /// </summary>
        Cascade = 1,

        /// <summary>
        /// All the values that make up the foreign key are set to NULL when the corresponding row in the parent table is updated.
        /// For this constraint to execute, the foreign key columns must be nullable.
        /// </summary>
        SetNull = 2,

        /// <summary>
        /// All the values that make up the foreign key are set to their default values when the corresponding row in the parent
        /// table is updated. For this constraint to execute, all foreign key columns must have default definitions.
        /// If a column is nullable, and there is no explicit default value set, NULL becomes the implicit default value
        /// of the column.
        /// </summary>
        SetDefault = 3
    }
}
