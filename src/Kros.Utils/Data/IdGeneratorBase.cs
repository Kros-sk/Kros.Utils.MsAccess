using Kros.Utils;
using System;
using System.Data.Common;

namespace Kros.Data
{
    /// <summary>
    /// Base class for simple creation of implementations of <see cref="IIdGenerator"/>.
    /// </summary>
    /// <seealso cref="IIdGenerator" />
    public abstract class IdGeneratorBase : IIdGenerator
    {
        private bool _disposeOfConnection = false;

        /// <summary>
        /// Creates an instance of ID generator for table <paramref name="tableName"/> in database <paramref name="connection"/>
        /// and with specified <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connection"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Value of <paramref name="batchSize"/> is less or equal than 0.</exception>
        public IdGeneratorBase(DbConnection connection, string tableName, int batchSize)
            : this(tableName, batchSize)
        {
            Connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Creates an instance of ID generator for table <paramref name="tableName"/> in database
        /// <paramref name="connectionString"/> and with specified <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connectionString"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="connectionString"/> is empty string, or string containing only
        /// whitespace characters.</item>
        /// <item>Value of <paramref name="batchSize"/> is less or equal than 0.</item>
        /// </list></exception>
        public IdGeneratorBase(string connectionString, string tableName, int batchSize)
            : this(tableName, batchSize)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Connection = CreateConnection(connectionString);
            _disposeOfConnection = true;
        }

        private IdGeneratorBase(string tableName, int batchSize)
        {
            TableName = Check.NotNull(tableName, nameof(tableName));
            BatchSize = Check.GreaterThan(batchSize, 0, nameof(batchSize));
        }

        /// <summary>
        /// Creates a database connection instance.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns>Specific instance of <see cref="DbConnection"/>.</returns>
        protected abstract DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Table name for which IDs are generated.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Batch size - saves roundtrips into database.
        /// </summary>
        public int BatchSize { get; }

        /// <summary>
        /// Database connection.
        /// </summary>
        protected DbConnection Connection { get; }

        private int _nextId = 0;
        private int _nextAccessToDb = -1;

        /// <inheritdoc cref="IIdGenerator.GetNext"/>
        public virtual int GetNext()
        {
            if (_nextAccessToDb <= _nextId)
            {
                _nextId = GetNewIdFromDb();
                _nextAccessToDb = _nextId + BatchSize;
            }

            return _nextId++;
        }

        private int GetNewIdFromDb()
        {
            using (ConnectionHelper.OpenConnection(Connection))
            {
                return GetNewIdFromDbCore();
            }
        }

        /// <summary>
        /// Returns new ID from database. In this method is ensured, that the <see cref="Connection"/> is opened.
        /// </summary>
        /// <returns>Next ID.</returns>
        protected abstract int GetNewIdFromDbCore();

        /// <inheritdoc/>
        public abstract void InitDatabaseForIdGenerator();

        #region IDisposable Support

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_disposeOfConnection)
                    {
                        Connection.Dispose();
                    }
                }
                _disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
