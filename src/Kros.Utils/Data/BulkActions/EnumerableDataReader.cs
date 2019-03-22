using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// <see cref="IBulkActionDataReader"/> implementation for any list of objects.
    /// </summary>
    /// <typeparam name="T">Object data type.</typeparam>
    /// <remarks>
    /// Class implements <see cref="IBulkActionDataReader"/> for any list of objects,
    /// so this list can be easily used in bulk actions (<see cref="IBulkInsert"/>, <see cref="IBulkUpdate"/>).
    /// </remarks>
    public class EnumerableDataReader<T> : IBulkActionDataReader
    {
        #region Fields

        private IEnumerator<T> _dataEnumerator;
        private readonly List<string> _columnNames;
        private readonly Dictionary<string, PropertyInfo> _propertyCache;
        private readonly bool _isPrimitiveType = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates instance of reader over <paramref name="data"/> with list of columns <paramref name="columnNames"/>.
        /// </summary>
        /// <param name="data">Data which reader operates with.</param>
        /// <param name="columnNames">List of columns with which reader works.
        /// For every column must exists property with the same name in object <c>T</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="data"/>, or <paramref name="columnNames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// List <paramref name="columnNames"/> is empty, i.e. does not contain any value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Class <c>T</c> does not have all properties defined in <paramref name="columnNames"/>.
        /// </exception>
        public EnumerableDataReader(IEnumerable<T> data, IEnumerable<string> columnNames)
        {
            Check.NotNull(data, nameof(data));
            Check.NotNull(columnNames, nameof(columnNames));

            _columnNames = columnNames.ToList();
            _isPrimitiveType = (typeof(T).IsPrimitive || typeof(T) == typeof(string));

            if (_isPrimitiveType)
            {
                Check.Equal(_columnNames.Count, 1, nameof(columnNames));
            }
            else
            {
                Check.GreaterOrEqualThan(_columnNames.Count, 1, nameof(columnNames));
                _propertyCache = LoadProperties(_columnNames);
            }

            _dataEnumerator = data.GetEnumerator();
        }

        #endregion

        #region IBulkActionDataReader

        /// <summary>
        /// Columns count.
        /// </summary>
        public int FieldCount => _columnNames.Count;

        /// <summary>
        /// Column name at index <paramref name="i"/>.
        /// </summary>
        /// <param name="i">Column index.</param>
        /// <returns>Column name.</returns>
        public string GetName(int i) => _columnNames[i];

        /// <summary>
        /// Index of column with <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <returns>Index.</returns>
        public int GetOrdinal(string name) => _columnNames.IndexOf(_columnNames.First(column => column == name));

        /// <summary>
        /// Returns value of column at index <paramref name="i"/>.
        /// </summary>
        /// <param name="i">Column index.</param>
        /// <returns>Column value.</returns>
        public object GetValue(int i)
        {
            if (_isPrimitiveType)
            {
                return _dataEnumerator.Current;
            }
            else
            {
                return _propertyCache[GetName(i)].GetValue(_dataEnumerator.Current, null);
            }
        }

        /// <inheritdoc cref="IBulkActionDataReader.GetString(int)"/>
        public string GetString(int i) => (string)GetValue(i);

        /// <inheritdoc cref="IBulkActionDataReader.IsDBNull(int)"/>
        public bool IsDBNull(int i)
        {
            object value = GetValue(i);
            return (value == null) || (value == DBNull.Value);
        }

        /// <summary>
        /// Moves to next record.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if move was successfull,
        /// <see langword="false"/> if there is no next record.
        /// </returns>
        public bool Read() => _dataEnumerator.MoveNext();

        #endregion

        #region Helpers

        private Dictionary<string, PropertyInfo> LoadProperties(IEnumerable<string> columnNames)
        {
            var properties = new Dictionary<string, PropertyInfo>();

            foreach (string columnName in columnNames)
            {
                properties[columnName] = typeof(T).GetProperty(columnName);
                if (properties[columnName] == null)
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.MissingPropertyInType, typeof(T).FullName, columnName));
                }
            }

            return properties;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dataEnumerator.Dispose();
                    _dataEnumerator = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
