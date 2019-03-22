using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kros.IO
{
    /// <summary>
    /// Helpers for working with file/folder paths.
    /// </summary>
    public static class PathHelper
    {
        #region Helpers

        private static readonly Regex _reReplacePathChars = new Regex(CreatePathReplacePattern(), RegexOptions.Compiled);

        private static string CreatePathReplacePattern()
        {
            HashSet<char> invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());
            invalidChars.UnionWith(Path.GetInvalidPathChars());

            System.Text.StringBuilder result = new System.Text.StringBuilder(invalidChars.Count + 3);
            result.Append("[");
            foreach (char c in invalidChars)
            {
                result.Append(Regex.Escape(Convert.ToString(c)));
            }
            result.Append("]+");

            return result.ToString();
        }

        #endregion

        /// <summary>
        /// Joins parts <paramref name="parts"/> to one string, representing path to a file/folder.
        /// </summary>
        /// <param name="parts">Path parts.</param>
        /// <returns>Created path.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="parts"/> or any of its item is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Any of the item in <paramref name="parts"/> contains invalid characters
        /// defined in <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// The method works similarly as standard .NET method <see cref="Path.Combine(string[])" autoUpgrade="true"/>,
        /// with some different details:
        /// <list type="bullet">
        /// <item>If any part begins with a slash (forward or backslash), this slash is ignored.
        /// The <see cref="Path.Combine(string[])" autoUpgrade="true"/> ignores all parts before the last part begining
        /// with a slash (if such exists).
        /// <example>
        /// <c>Path.Combine("lorem", "\ipsum", "dolor")</c> returns <c>\ipsum\dolor</c><br />
        /// <c>PathHelper.BuildPath("lorem", "\ipsum", "dolor")</c> returns <c>lorem\ipsum\dolor</c>
        /// </example>
        /// </item>
        /// <item>If any part ends with a disk separator (<c>:</c>), directory separator is appended even after it.
        /// <example>
        /// <c>Path.Combine("c:", "lorem", "ipsum", "dolor")</c> returns <b>c:lorem\ipsum\dolor</b><br />
        /// <c>PathHelper.Build("c:", "lorem", "ipsum", "dolor")</c> returns <b>c:\lorem\ipsum\dolor</b>
        /// </example>
        /// Some of the .NET function are not able to work with a paths like <c>c:lorem</c> and the throw exceptions.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static string BuildPath(params string[] parts)
        {
            Check.NotNull(parts, nameof(parts));

            int capacity = CheckBuildPathParts(parts);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
            foreach (string part in parts)
            {
                if (part.Length > 0)
                {
                    if (sb.Length > 0)
                    {
                        char firstChar = part[0];
                        char lastChar = sb[sb.Length - 1];

                        if (((firstChar == Path.DirectorySeparatorChar) || (firstChar == Path.AltDirectorySeparatorChar)) &&
                            ((lastChar == Path.DirectorySeparatorChar) || (lastChar == Path.AltDirectorySeparatorChar)))
                        {
                            sb.Length -= 1;
                        }
                        else if ((firstChar != Path.DirectorySeparatorChar) && (firstChar != Path.AltDirectorySeparatorChar) &&
                            (lastChar != Path.DirectorySeparatorChar) && (lastChar != Path.AltDirectorySeparatorChar))
                        {
                            sb.Append(Path.DirectorySeparatorChar);
                        }
                    }
                    sb.Append(part);
                }
            }

            return sb.ToString();
        }

        private static int CheckBuildPathParts(string[] parts)
        {
            int capacity = parts.Length;
            int partIndex = 0;
            char[] invalidChars = Path.GetInvalidPathChars();

            foreach (string part in parts)
            {
                Check.NotNull(part, nameof(parts));

                if (part.IndexOfAny(invalidChars) >= 0)
                {
                    throw new ArgumentException(string.Format(Resources.PathContainsInvalidCharacters, partIndex, part));
                }

                capacity += part.Length;
                partIndex++;
            }
            return capacity;
        }

        /// <summary>
        /// Replaces invalid characters in <paramref name="pathName"/> with dash (<c>-</c>). If there are
        /// several succesive invalid characters, they all are replaced only by one dash.
        /// </summary>
        /// <param name="pathName">Input path.</param>
        /// <remarks><inheritdoc cref="ReplaceInvalidPathChars(string, string)"/></remarks>
        /// <returns><inheritdoc cref="ReplaceInvalidPathChars(string, string)"/></returns>
        public static string ReplaceInvalidPathChars(string pathName)
        {
            return ReplaceInvalidPathChars(pathName, "-");
        }

        /// <summary>
        /// Replaces invalid characters in <paramref name="pathName"/> with <paramref name="replacement"/>. If there are
        /// several succesive invalid characters, they all are replaced only by one <paramref name="replacement"/>.
        /// </summary>
        /// <param name="pathName">Input path.</param>
        /// <param name="replacement">Value, by which are replaced invalid path charactes. If the value is <see langword="null"/>,
        /// empty stirng is used, so invalid characters are removed.</param>
        /// <remarks>
        /// Replaced are all characters in <see cref="Path.GetInvalidFileNameChars"/> and <see cref="Path.GetInvalidPathChars"/>.
        /// </remarks>
        /// <returns>
        /// String with invalid path characters replaced. If input <paramref name="pathName"/> is <see langword="null"/>,
        /// empty string is returned.
        /// </returns>
        public static string ReplaceInvalidPathChars(string pathName, string replacement)
        {
            if (pathName == null)
            {
                return string.Empty;
            }
            if (replacement == null)
            {
                replacement = string.Empty;
            }
            return _reReplacePathChars.Replace(pathName, replacement);
        }

        /// <summary>
        /// Returns path to system temporary folder (<see cref="Path.GetTempPath"/>) <b>without</b> trailing directory separator.
        /// </summary>
        public static string GetTempPath()
        {
            return Path.GetTempPath().TrimEnd(new char[] { Path.DirectorySeparatorChar });
        }

        /// <summary>
        /// Checks, if specified <paramref name="path"/> is network share path. The path is considered network share path,
        /// if it begins with <c>\\</c>.
        /// </summary>
        /// <param name="path">Checked path.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="path"/> is network share path, <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsNetworkPath(string path)
        {
            return (GetDriveTypeFromPath(path) == DriveType.Network);
        }

        private static DriveType GetDriveTypeFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return DriveType.Unknown;
            }
            if ((path.Length >= 2) && (path[0] == '\\') && (path[1] == '\\'))
            {
                return DriveType.Network;
            }

            string driveName = path.Length > 3 ? path.Substring(0, 3) : path;
            DriveInfo drive = DriveInfo.GetDrives()
                .Where(item => item.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (drive != null)
            {
                return drive.DriveType;
            }

            return DriveType.Unknown;
        }
    }
}
