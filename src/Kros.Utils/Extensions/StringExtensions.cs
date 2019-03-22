using Kros.Utils;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kros.Extensions
{
    /// <summary>
    /// General extension methods for strings (<see cref="System.String"/>).
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks, if string <paramref name="value"/> is <see langword="null"/>, or empty string (<c>string.Empty</c>).
        /// </summary>
        /// <param name="value">Checked stirng.</param>
        /// <returns><see langword="true"/>, if <paramref name="value"/> is <see langword="null"/> or <c>string.Empty</c>,
        /// <see langword="false"/> otherwise.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Checks, if string <paramref name="value"/> is <see langword="null"/>, empty string (<c>string.Empty</c>),
        /// or string containig only whitespace characters.
        /// </summary>
        /// <param name="value">Checked string.</param>
        /// <returns><see langword="true"/>, if <paramref name="value"/> is <see langword="null"/>, empty string, or string
        /// containing only white characters, <see langword="false"/> otherwise.</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Returns the same string without diacritic marks (for example <c>čšá</c> becomes <c>csa</c>).
        /// </summary>
        public static string RemoveDiacritics(this string value)
        {
            string normalizedString = value.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(normalizedString.Length);

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static readonly Regex _reRemoveNewLines = new Regex(@"[\n\r]");

        /// <summary>
        /// Removes new line characters from string. Removed characters are <c>line feed</c> (<c>\n</c>) and
        /// <c>carriage return</c> (<c>\r</c>).
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <returns>String without new line characters, or <see langword="null"/> if <paramref name="value"/>
        /// is <see langword="null"/>.</returns>
        public static string RemoveNewLines(this string value)
        {
            return value == null ? null : _reRemoveNewLines.Replace(value, string.Empty);
        }

        /// <summary>
        /// Returns first <paramref name="length"/> of characters form input string <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="length">Number of characters at the begining of <paramref name="value"/> which are returned.</param>
        /// <returns>Begining of the input string <paramref name="value"/> of length <paramref name="length"/>.
        /// If <paramref name="value"/> is <see langword="null"/>, empty string is returned. If value of <paramref name="length"/>
        /// is greater than length of <paramref name="value"/>, the <paramref name="value"/> itself is returned.</returns>
        /// <exception cref="System.ArgumentException">Value of <paramref name="length"/> is less than 0.</exception>
        public static string Left(this string value, int length)
        {
            Check.GreaterOrEqualThan(length, 0, nameof(length));

            if (value == null)
            {
                return string.Empty;
            }
            else if (length >= value.Length)
            {
                return value;
            }
            return value.Substring(0, length);
        }

        /// <summary>
        /// Returns last <paramref name="length"/> of characters form input string <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Input string.</param>
        /// <param name="length">Number of characters at the end of <paramref name="value"/> which are returned.</param>
        /// <returns>End of the input string <paramref name="value"/> of length <paramref name="length"/>.
        /// If <paramref name="value"/> is <see langword="null"/>, empty string is returned. If value of <paramref name="length"/>
        /// is greater than length of <paramref name="value"/>, the <paramref name="value"/> itself is returned.</returns>
        /// <exception cref="System.ArgumentException">Value of <paramref name="length"/> is less than 0.</exception>
        public static string Right(this string value, int length)
        {
            Check.GreaterOrEqualThan(length, 0, nameof(length));

            if (value == null)
            {
                return string.Empty;
            }
            else if (length >= value.Length)
            {
                return value;
            }
            return value.Substring(value.Length - length);
        }

        /// <inheritdoc cref="System.String.Format(string, object)"/>
        public static string Format(this string format, object arg0) => string.Format(format, arg0);

        /// <inheritdoc cref="System.String.Format(string, object, object)"/>
        public static string Format(this string format, object arg0, object arg1) => string.Format(format, arg0, arg1);

        /// <inheritdoc cref="System.String.Format(string, object, object, object)"/>
        public static string Format(this string format, object arg0, object arg1, object arg2)
            => string.Format(format, arg0, arg1, arg2);

        /// <inheritdoc cref="System.String.Format(string, object[])"/>
        public static string Format(this string format, params object[] args)
            => string.Format(format, args);

        /// <inheritdoc cref="System.String.Format(IFormatProvider, string, object)"/>
        public static string Format(this string format, IFormatProvider provider, object arg0)
            => string.Format(provider, format, arg0);

        /// <inheritdoc cref="System.String.Format(IFormatProvider, string, object, object)"/>
        public static string Format(this string format, IFormatProvider provider, object arg0, object arg1)
            => string.Format(provider, format, arg0, arg1);

        /// <inheritdoc cref="System.String.Format(IFormatProvider, string, object, object, object)"/>
        public static string Format(this string format, IFormatProvider provider, object arg0, object arg1, object arg2)
            => string.Format(provider, format, arg0, arg1, arg2);

        /// <inheritdoc cref="System.String.Format(IFormatProvider, string, object[])"/>
        public static string Format(this string format, IFormatProvider provider, params object[] args)
            => string.Format(provider, format, args);
    }
}
