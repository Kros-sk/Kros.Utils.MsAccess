using Kros.Data;
using Kros.Data.MsAccess;
using Kros.MsAccess.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace Kros.UnitTests
{
    /// <summary>
    /// Helper class for unit tests, if there is a need for real database in tests. It creates temporary empty database
    /// for testing. This database is automatically deleted after finishing the work.
    /// </summary>
    public class MsAccessTestHelper
        : IDisposable
    {
        #region Fields

        private readonly ProviderType _provider;
        private readonly Stream _sourceDatabaseStream = null;
        private readonly string _sourceDatabasePath = null;
        private string _databasePath;
        private OleDbConnection _connection = null;
        private readonly IEnumerable<string> _initDatabaseScripts = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as an empty database
        /// with random name in temporary folder.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        public MsAccessTestHelper(ProviderType provider)
            : this(provider, null as IEnumerable<string>)
        {
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as an empty database
        /// with random name in temporary folder and is initialized with scripts in <paramref name="initDatabaseScripts"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="initDatabaseScripts">List of scripts, which are used for database initialization.
        /// For example, they can be scripts for creating and filling necessary tables.</param>
        public MsAccessTestHelper(ProviderType provider, IEnumerable<string> initDatabaseScripts)
        {
            _provider = provider;
            _initDatabaseScripts = initDatabaseScripts;
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabasePath"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabasePath">Source database. New temporary database is created as a copy of this.</param>
        public MsAccessTestHelper(ProviderType provider, string sourceDatabasePath)
            : this(provider, sourceDatabasePath, null as IEnumerable<string>)
        {
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabasePath"/> and initialized with script <paramref name="initDatabaseScript"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabasePath">Source database. New temporary database is created as a copy of this.</param>
        /// <param name="initDatabaseScript">A script, which is used for database initialization.
        /// For example, it can be script for creating some table.</param>
        public MsAccessTestHelper(ProviderType provider, string sourceDatabasePath, string initDatabaseScript)
            : this(provider, sourceDatabasePath,
                  string.IsNullOrWhiteSpace(initDatabaseScript) ? null : new string[] { initDatabaseScript })
        {
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabasePath"/> and initialized with scripts in <paramref name="initDatabaseScripts"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabasePath">Source database. New temporary database is created as a copy of this.</param>
        /// <param name="initDatabaseScripts">List of scripts, which are used for database initialization.
        /// For example, they can be scripts for creating and filling necessary tables.</param>
        public MsAccessTestHelper(ProviderType provider, string sourceDatabasePath, IEnumerable<string> initDatabaseScripts)
        {
            _sourceDatabasePath = Check.NotNullOrWhiteSpace(sourceDatabasePath, nameof(sourceDatabasePath));
            _provider = provider;
            _initDatabaseScripts = initDatabaseScripts;
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabaseStream"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabaseStream">Source database. New temporary database is created as a copy of this.</param>
        public MsAccessTestHelper(ProviderType provider, Stream sourceDatabaseStream)
            : this(provider, sourceDatabaseStream, null as IEnumerable<string>)
        {
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabaseStream"/> and initialized with script <paramref name="initDatabaseScript"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabaseStream">Source database. New temporary database is created as a copy of this.</param>
        /// <param name="initDatabaseScript">A script, which is used for database initialization.
        /// For example, it can be script for creating some table.</param>
        public MsAccessTestHelper(ProviderType provider, Stream sourceDatabaseStream, string initDatabaseScript)
            : this(provider, sourceDatabaseStream,
                  string.IsNullOrWhiteSpace(initDatabaseScript) ? null : new string[] { initDatabaseScript })
        {
        }

        /// <summary>
        /// Creates an instance of helper with specified parameters. New database file is created as a copy of
        /// <paramref name="sourceDatabaseStream"/> and initialized with scripts in <paramref name="initDatabaseScripts"/>.
        /// </summary>
        /// <param name="provider">Microsoft Access provider type.</param>
        /// <param name="sourceDatabaseStream">Source database. New temporary database is created as a copy of this.</param>
        /// <param name="initDatabaseScripts">List of scripts, which are used for database initialization.
        /// For example, they can be scripts for creating and filling necessary tables.</param>
        public MsAccessTestHelper(ProviderType provider, Stream sourceDatabaseStream, IEnumerable<string> initDatabaseScripts)
        {
            _sourceDatabaseStream = Check.NotNull(sourceDatabaseStream, nameof(sourceDatabaseStream));
            _provider = provider;
            _initDatabaseScripts = initDatabaseScripts;
        }

        #endregion

        #region Test helpers

        /// <summary>
        /// Path to created temporary database.
        /// </summary>
        public string DatabasePath => _databasePath;

        /// <summary>
        /// Connection to created temporary database.
        /// </summary>
        public OleDbConnection Connection
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
        /// Generates path to the file where database will be created. Default is random filename in system's temp folder.
        /// </summary>
        /// <returns>Path to the database file.</returns>
        protected virtual string GenerateDatabaseName() => Path.GetTempFileName();

        /// <summary>
        /// Initializes new empty database. Method is called after empty temporary database is created and it executes
        /// initialization scripts (specified in constructor). method is called only once.
        /// </summary>
        protected virtual void InitDatabase()
        {
            if (_initDatabaseScripts != null)
            {
                using (ConnectionHelper.OpenConnection(Connection))
                using (OleDbCommand cmd = Connection.CreateCommand())
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

        private void CreateConnection() => _connection = new OleDbConnection(InitConnectionString(_provider));

        private string InitConnectionString(ProviderType provider)
        {
            if (!MsAccessDataHelper.HasProvider(provider))
            {
                throw new InvalidOperationException(string.Format(Resources.ProviderNotInstaller, provider.ToString()));
            }

            _databasePath = GenerateDatabaseName();
            if (!string.IsNullOrEmpty(_sourceDatabasePath))
            {
                File.Copy(_sourceDatabasePath, _databasePath, true);
            }
            else if (_sourceDatabaseStream != null)
            {
                using (FileStream writer = new FileStream(_databasePath, FileMode.Create, FileAccess.Write))
                {
                    _sourceDatabaseStream.CopyTo(writer);
                }
            }
            else
            {
                MsAccessDataHelper.CreateEmptyDatabase(_databasePath, provider);
            }
            return MsAccessDataHelper.CreateConnectionString(_databasePath, provider);
        }

        private void RemoveDatabase()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
                if (File.Exists(_databasePath))
                {
                    File.Delete(_databasePath);
                }
                string folder = Path.GetDirectoryName(_databasePath);
                string fileName = Path.GetFileNameWithoutExtension(_databasePath);
                string ldbFilePath = Path.Combine(folder, fileName + ".ldb");
                if (File.Exists(ldbFilePath))
                {
                    File.Delete(ldbFilePath);
                }
            }
        }

        #endregion

        #region IDisposable

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Dispose() => RemoveDatabase();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
