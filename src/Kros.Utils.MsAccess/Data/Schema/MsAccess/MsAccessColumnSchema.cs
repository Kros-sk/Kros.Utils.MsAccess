using Kros.Utils;
using System;
using System.Data;
using System.Data.OleDb;

namespace Kros.Data.Schema.MsAccess
{
    /// <summary>
    /// Table's column schema for Microsoft Access.
    /// </summary>
    public class MsAccessColumnSchema
        : ColumnSchema
    {
        #region Constructors

        /// <inheritdoc/>
        public MsAccessColumnSchema(string name)
            : this(name, DefaultAllowNull, DefaultDefaultValue, DefaultSize)
        {
        }

        /// <inheritdoc/>
        public MsAccessColumnSchema(string name, bool allowNull)
            : this(name, allowNull, DefaultDefaultValue, DefaultSize)
        {
        }

        /// <inheritdoc/>
        public MsAccessColumnSchema(string name, bool allowNull, object defaultValue)
            : this(name, allowNull, defaultValue, DefaultSize)
        {
        }

        /// <inheritdoc/>
        public MsAccessColumnSchema(string name, bool allowNull, object defaultValue, int size)
            : base(name, allowNull, defaultValue, size)
        {
        }

        #endregion

        #region Common

        /// <summary>
        /// Data type of the column.
        /// </summary>
        public OleDbType OleDbType { get; set; }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Hodnota <paramref name="param"/> nie je typu <see cref="OleDbParameter"/>.
        /// </exception>
        public override void SetupParameter(IDataParameter param)
        {
            Check.IsOfType<OleDbParameter>(param, nameof(param));
            var oleParam = (OleDbParameter)param;
            oleParam.OleDbType = OleDbType;
            oleParam.Precision = Precision;
            oleParam.Scale = Scale;
            if (Size > 0)
            {
                oleParam.Size = Size;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            return string.Format("Column {0}: OleDbType = {1}, AllowNull = {2}, DefaultValue = {3}, Size = {4}",
                FullName, OleDbType, AllowNull, ToStringDefaultValue(), Size);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
