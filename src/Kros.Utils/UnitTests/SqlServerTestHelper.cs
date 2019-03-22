using Kros.Data;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Kros.UnitTests
{
    /// <summary>
    /// Helper class for unit testing using real SQL Server database.
    /// </summary>
    /// <remarks>
    /// In general, the unit tests should not require real database. But in some cases, this is necessary. This class
    /// manages creation of <i>temporary</i> database, which the tests will use. Database name is generated to be unique
    /// and after finishing (<see cref="Dispose()"/>), the database is deleted. Connection to created database is
    /// available in <see cref="Connection"/> property.
    /// </remarks>
    /// <example>
    /// <code language = "cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\SqlServerTestHelperExamples.cs" region="SqlServerTestHelper" />
    /// </example>
    public class SqlServerTestHelper
        : IDisposable
    {
        #region Constants

        private const string MasterDatabaseName = "master";

        #endregion

        #region Fields

        private SqlConnection _connection = null;
        private readonly IEnumerable<string> _initDatabaseScripts = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of helper with connection <paramref name="baseConnectionString"/> and base database name
        /// <paramref name="baseDatabaseName"/>.
        /// </summary>
        /// <param name="baseConnectionString">Base connection string to SQL Server, where database will be created.</param>
        /// <param name="baseDatabaseName">Base database name. GUID will be appended to it. The value is not required.</param>
        public SqlServerTestHelper(string baseConnectionString, string baseDatabaseName)
            : this(baseConnectionString, baseDatabaseName, null as IEnumerable<string>)
        {
        }

        /// <summary>
        /// Creates an instance of helper with connection <paramref name="baseConnectionString"/> and base database name
        /// <paramref name="baseDatabaseName"/>. Created database will be initialized with script
        /// <paramref name="initDatabaseScript"/>.
        /// </summary>
        /// <param name="baseConnectionString">Base connection string to SQL Server, where database will be created.</param>
        /// <param name="baseDatabaseName">Base database name. GUID will be appended to it. The value is not required.</param>
        /// <param name="initDatabaseScript">The script, which is executed when database is created. For example,
        /// it can be script to create some table.</param>
        public SqlServerTestHelper(string baseConnectionString, string baseDatabaseName, string initDatabaseScript)
            : this(baseConnectionString, baseDatabaseName,
                  string.IsNullOrWhiteSpace(initDatabaseScript) ? null : new string[] { initDatabaseScript })
        {
        }

        /// <summary>
        /// Creates an instance of helper with connection <paramref name="baseConnectionString"/> and base database name
        /// <paramref name="baseDatabaseName"/>. Created database will be initialized with scripts from
        /// <paramref name="initDatabaseScripts"/>.
        /// </summary>
        /// <param name="baseConnectionString">Base connection string to SQL Server, where database will be created.</param>
        /// <param name="baseDatabaseName">Base database name. GUID will be appended to it. The value is not required.</param>
        /// <param name="initDatabaseScripts">List of scripts, which are executed when database is created. For example,
        /// they can be scripts to create necessary tables and data.</param>
        public SqlServerTestHelper(string baseConnectionString, string baseDatabaseName, IEnumerable<string> initDatabaseScripts)
        {
            BaseConnectionString = Check.NotNullOrWhiteSpace(baseConnectionString, nameof(baseConnectionString));

            BaseDatabaseName = baseDatabaseName?.Trim();
            _initDatabaseScripts = initDatabaseScripts;
        }

        #endregion

        #region Test helpers

        /// <summary>
        /// Base connection string to SQL Server, where temporary database will be created. Database name does not need
        /// to be set in connection stirng, because it will be generated.
        /// </summary>
        public string BaseConnectionString { get; }

        /// <summary>
        /// Base database name. GUID is appended to this name, to make database name unique. If <c>BaseDatabaseName</c>
        /// is empty, the database name will be just that GUID.
        /// </summary>
        public string BaseDatabaseName { get; }

        /// <summary>
        /// Connection to created database.
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    CreateDatabase();
                }
                return _connection;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Generates a name for database, which is created on SQL Server. Returned name is composed from
        /// <see cref="BaseDatabaseName"/> (if specified) and generated GUID, to make it unique.
        /// </summary>
        /// <returns>Database name.</returns>
        protected virtual string GenerateDatabaseName()
        {
            string unique = Guid.NewGuid().ToString();
            return string.IsNullOrWhiteSpace(BaseDatabaseName) ? unique : $"{BaseDatabaseName}_{unique}";
        }

        /// <summary>
        /// Initializes a database. Method is executed once after creating the database and it executes scripts
        /// which were specified in constructor.
        /// </summary>
        protected virtual void InitDatabase()
        {
            if (_initDatabaseScripts != null)
            {
                using (ConnectionHelper.OpenConnection(Connection))
                using (SqlCommand cmd = Connection.CreateCommand())
                {
                    foreach (string script in _initDatabaseScripts)
                    {
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void CreateDatabase()
        {
            if (_connection == null)
            {
                CreateConnection();
                InitDatabase();
            }
        }

        private void CreateConnection()
        {
            string databaseName = GenerateDatabaseName();
            using (SqlConnection masterConnection = GetConnectionCore(MasterDatabaseName))
            using (ConnectionHelper.OpenConnection(masterConnection))
            {
                using (SqlCommand cmd = masterConnection.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE [{databaseName}]";
                    cmd.ExecuteNonQuery();
                }
            }
            _connection = GetConnectionCore(databaseName);
        }

        private SqlConnection GetConnectionCore(string databaseName)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(BaseConnectionString)
            {
                InitialCatalog = databaseName,
                Pooling = false,
                PersistSecurityInfo = true,
                ConnectTimeout = 4
            };

            return new SqlConnection(builder.ToString());
        }

        private void RemoveDatabase()
        {
            if (_connection != null)
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connection.ConnectionString);
                _connection.Dispose();
                _connection = null;
                using (SqlConnection connection = GetConnectionCore(MasterDatabaseName))
                using (ConnectionHelper.OpenConnection(connection))
                {
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"DROP DATABASE [{builder.InitialCatalog}]";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
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
                    RemoveDatabase();
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
