using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kros.Data.Schema
{
    /// <summary>
    /// Standard parsers for columns' default values. The default values in database are stored as string and it is
    /// necessary to convert them to the specific column's type. If conversion to desired type fails, the returned
    /// value is always <see langword="null"/>.
    /// </summary>
    public static class DefaultValueParsers
    {
        /// <summary>
        /// Delegate for function which parses default value.
        /// </summary>
        /// <param name="defaultValue">Column's default value as string.</param>
        /// <returns>Returns value converted to desired data type, or <see langword="null"/> if conversion failed.</returns>
        public delegate object ParseDefaultValueFunction(string defaultValue);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static object ParseInt64(string defaultValue)
        {
            if (long.TryParse(defaultValue, out long value))
            {
                return value;
            }
            return null;
        }

        public static object ParseInt32(string defaultValue)
        {
            if (int.TryParse(defaultValue, out int value))
            {
                return value;
            }
            return null;
        }

        public static object ParseInt16(string defaultValue)
        {
            if (short.TryParse(defaultValue, out short value))
            {
                return value;
            }
            return null;
        }

        public static object ParseByte(string defaultValue)
        {
            if (byte.TryParse(defaultValue, out byte value))
            {
                return value;
            }
            return null;
        }

        public static object ParseUInt64(string defaultValue)
        {
            if (ulong.TryParse(defaultValue, out ulong value))
            {
                return value;
            }
            return null;
        }

        public static object ParseUInt32(string defaultValue)
        {
            if (uint.TryParse(defaultValue, out uint value))
            {
                return value;
            }
            return null;
        }

        public static object ParseUInt16(string defaultValue)
        {
            if (ushort.TryParse(defaultValue, out ushort value))
            {
                return value;
            }
            return null;
        }

        public static object ParseSByte(string defaultValue)
        {
            if (sbyte.TryParse(defaultValue, out sbyte value))
            {
                return value;
            }
            return null;
        }

        public static object ParseDecimal(string defaultValue)
        {
            if (decimal.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out decimal value))
            {
                return value;
            }
            return null;
        }

        public static object ParseDouble(string defaultValue)
        {
            if (double.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out double value))
            {
                return value;
            }
            return null;
        }

        public static object ParseSingle(string defaultValue)
        {
            if (float.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float value))
            {
                return value;
            }
            return null;
        }

        public static object ParseBool(string defaultValue)
        {
            if (int.TryParse(defaultValue, out int parsedInt))
            {
                return (parsedInt != 0);
            }
            else if (bool.TryParse(defaultValue, out bool parsedBool))
            {
                return parsedBool;
            }
            else if (defaultValue.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return null;
        }

        public static object ParseGuid(string defaultValue)
        {
            if (Guid.TryParse(defaultValue, out Guid value))
            {
                return value;
            }
            return null;
        }

        public static object ParseDate(string defaultValue)
        {
            if (defaultValue.StartsWith("#") && defaultValue.EndsWith("#"))
            {
                // Date in format #month/day/year# - i. e. #12/31/2107#
                int year = int.Parse(defaultValue.Substring(7, 4));
                int month = int.Parse(defaultValue.Substring(1, 2));
                int day = int.Parse(defaultValue.Substring(4, 2));
                return new DateTime(year, month, day);
            }
            return null;
        }

        public static object ParseDateSql(string defaultValue)
        {
            if (DateTime.TryParse(defaultValue, out DateTime value))
            {
                return value;
            }
            return null;
        }

        public static object ParseDateTimeOffsetSql(string defaultValue)
        {
            if (DateTimeOffset.TryParse(defaultValue, out DateTimeOffset value))
            {
                return value;
            }
            return null;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
