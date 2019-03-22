using Kros.Utils;
using System;
using System.Data;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Schema of a database table column.
    /// </summary>
    public abstract class ColumnSchema
    {
        #region Constants

        /// <summary>
        /// Value for the column's <see cref="DefaultValue"/>, if none is defined. The value is <see cref="DBNull"/>.
        /// </summary>
        public static readonly object DefaultDefaultValue = DBNull.Value;

        /// <summary>
        /// Default value for column's <see cref="AllowNull"/>. The value is <see langword="false"/>.
        /// </summary>
        public const bool DefaultAllowNull = true;

        /// <summary>
        /// Default value for column's <see cref="Size"/>. The value is <c>0</c>.
        /// </summary>
        public const int DefaultSize = 0;

        /// <summary>
        /// Default value for column's <see cref="Precision"/>. The value is <c>0</c>.
        /// </summary>
        public const byte DefaultPrecision = 0;

        /// <summary>
        /// Default value for column's <see cref="Scale"/>. The value is <c>0</c>.
        /// </summary>
        public const byte DefaultScale = 0;

        /// <summary>
        /// Columns' default values for individual data types.
        /// <list type="bullet">
        /// <item><c>Boolean</c>'s default value is <see langword="false"/>.</item>
        /// <item>Default value for all numeric types is <c>0</c>.</item>
        /// <item>Default value for date and time is <c>1.1.1900 0:00:00</c></item>
        /// <item>String's default value is empty string.</item>
        /// <item>GUID's default value is empty GUID (<see cref="Guid.Empty"/>).</item>
        /// </list>
        /// </summary>
        public static class DefaultValues
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public const bool Boolean = false;
            public const sbyte SByte = 0;
            public const short Int16 = 0;
            public const int Int32 = 0;
            public const long Int64 = 0L;
            public const byte Byte = 0;
            public const ushort UInt16 = 0;
            public const uint UInt32 = 0;
            public const ulong UInt64 = 0L;
            public const float Single = 0.0F;
            public const double Double = 0.0;
            public const decimal Decimal = 0;
            public const string Text = "";
            public static readonly Guid Guid = Guid.Empty;
            public static readonly DateTime DateTime = new DateTime(1900, 1, 1);
            public static readonly DateTime Date = DefaultValues.DateTime;
            public static readonly DateTime Time = DefaultValues.DateTime;
            public static readonly DBNull Null = DBNull.Value;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of column schema with <paramref name="name"/> and specified parameters.
        /// </summary>
        /// <param name="name">Column's name</param>
        /// <param name="allowNull">Specifies if column accepts <b>NULL</b> value.</param>
        /// <param name="defaultValue">Column's default value.</param>
        /// <param name="size">Maximum length of text columns. If value is <b>0</b>, maximum length is unlimited.</param>
        /// <exception cref="ArgumentNullException">Value of <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="name"/> is empty string, or string containing only
        /// whitespace characters.</exception>
        public ColumnSchema(string name, bool allowNull, object defaultValue, int size)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));

            AllowNull = allowNull;
            DefaultValue = defaultValue;
            Size = size;
        }

        #endregion

        #region Common

        /// <summary>
        /// The table to which the column belongs. The table is set automatically when the column is added to table's
        /// <see cref="TableSchema.Columns"/> collection.
        /// </summary>
        public TableSchema Table { get; internal set; }

        /// <summary>
        /// Column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Full name of the column, together with the table name (if column belongs to table).
        /// </summary>
        public string FullName { get { return (Table == null) ? Name : $"{Table.Name}.{Name}"; } }

        /// <summary>
        /// Specifies if <b>NULL</b> value is allowed.
        /// </summary>
        public bool AllowNull { get; set; } = DefaultAllowNull;

        /// <summary>
        /// Column's default value.
        /// </summary>
        public object DefaultValue { get; set; } = DefaultDefaultValue;

        /// <summary>
        /// Maximum length of text columns. If value is <b>0</b>, maximum length is unlimited.
        /// </summary>
        public int Size { get; set; } = DefaultSize;

        /// <summary>
        /// The maximum total number of decimal digits that will be stored, both to the left and to the right
        /// of the decimal point.
        /// </summary>
        public byte Precision { get; set; } = DefaultPrecision;

        /// <summary>
        /// The number of decimal digits that will be stored to the right of the decimal point.
        /// This number is subtracted from p to determine the maximum number of digits to the left of the decimal point.
        /// </summary>
        public byte Scale { get; set; } = DefaultScale;

        /// <summary>
        /// Sets-up command parameter <paramref name="param"/> according to the column schema.
        /// </summary>
        /// <param name="param">Parameter for <see cref="IDbCommand"/> commands.</param>
        /// <remarks>Method should set parameters data type, and other specific values (size for <c>VARCHAR</c>,
        /// precision and scale for float columns).</remarks>
        public abstract void SetupParameter(IDataParameter param);

        /// <summary>
        /// Returns value for <see cref="DefaultValue"/> for use in <c>ToString()</c>.
        /// </summary>
        /// <returns>String "<c>NULL</c>" if value of <see cref="DefaultValue"/> is <see cref="DBNull"/> or
        /// <see langword="null"/>. Otherwise returns <see cref="DefaultValue"/>.</returns>
        protected object ToStringDefaultValue()
        {
            return (DefaultValue == DBNull.Value) || (DefaultValue == null) ? "NULL" : DefaultValue;
        }

        #endregion
    }
}
