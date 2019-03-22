using System;
using System.Collections.Generic;

namespace Kros.UnitTests
{
    /// <summary>
    /// Base class for database integration tests on Microsoft SQL Server. The class takes care of creating and initialization
    /// of database. Inherited classes just use connection to this database.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Database with unique name is created at the begining and it is deleted when <see cref="Dispose()"/> is called.
    /// The created database may be initielized with own scripts in <see cref="DatabaseInitScripts"/>.
    /// </para>
    /// <para>
    /// Descendant classes must override <see cref="BaseConnectionString"/> to set the connection to SQL Server.
    /// </para>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\SqlServerDatabaseTestBaseExamples.cs" region="SqlServerDatabaseTestBase"/>
    /// </remarks>
    public abstract class SqlServerDatabaseTestBase
        : IDisposable
    {
        private readonly SqlServerTestHelper _serverHelper;

        /// <summary>
        /// Creates an instance of <c>SqlServerDatabaseTestBase</c>.
        /// </summary>
        public SqlServerDatabaseTestBase()
        {
            _serverHelper = new SqlServerTestHelper(BaseConnectionString, BaseDatabaseName, DatabaseInitScripts);
        }

        /// <summary>
        /// Base database name. GUID is appended to this name to make it unique.
        /// Default implementation returns class full name (<c>GetType().FullName</c>) with underscore (<c>_</c>) appended.
        /// </summary>
        /// <seealso cref="SqlServerTestHelper.BaseDatabaseName"/>
        /// <seealso cref="SqlServerTestHelper"/>
        protected virtual string BaseDatabaseName => GetType().FullName.Replace("+", "__") + "_";

        /// <summary>
        /// Base connection string to SQL Server, where database will be created. It does not need to have database name,
        /// because it will be generated to make it unique.
        /// </summary>
        /// <seealso cref="SqlServerTestHelper"/>
        /// <seealso cref="SqlServerTestHelper.BaseConnectionString"/>
        protected abstract string BaseConnectionString { get; }

        /// <summary>
        /// SQL scripts for initializing created database.
        /// </summary>
        /// <remarks>
        /// The class creates a database for tests using <see cref="SqlServerTestHelper"/>. If it is necessary to have
        /// this database initialized (tables, data...), this is the list of scripts for it.
        /// </remarks>
        protected virtual IEnumerable<string> DatabaseInitScripts => null;

        /// <summary>
        /// Helper for accessing database using its <see cref="SqlServerTestHelper.Connection"/> property.
        /// </summary>
        protected SqlServerTestHelper ServerHelper
        {
            get
            {
                CheckDisposed();
                return _serverHelper;
            }
        }

        /// <summary>
        /// Checks, if the instance was disposed of (<see cref="Dispose()"/> was called). If yes, it throws
        /// <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the instance was already disposed of.</exception>
        protected void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #region IDisposable Support

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _serverHelper.Dispose();
                }
                _disposedValue = true;
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
