using Kros.MsAccess.Properties;
using Kros.Utils;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace Kros.Data.MsAccess
{
    /// <summary>
    /// General helpers for work Microsoft Access.
    /// </summary>
    public static class MsAccessDataHelper
    {
        /// <summary>
        /// Identification of Microsoft Access classes (used for example in <see cref="MsAccessIdGeneratorFactory"/>,
        /// <see cref="BulkActions.MsAccess.MsAccessBulkActionFactory"/>).
        /// </summary>
        public const string ClientId = "System.Data.OleDb";

        /// <summary>
        /// Identification of Microsoft Access ACE provider: <c>Microsoft.ACE.OLEDB</c>.
        /// </summary>
        public const string AceProviderBase = "Microsoft.ACE.OLEDB";

        /// <summary>
        /// Identification of Microsoft Access JET provider: <c>Microsoft.Jet.OLEDB</c>.
        /// </summary>
        public const string JetProviderBase = "Microsoft.Jet.OLEDB";

        private const string BaseConnectionString = "Provider={0};Data Source={1};";

        private const string ResourcesNamespace = "Kros.MsAccess.Resources";
        private const string AccdbResourceName = "EmptyDatabase.accdb";
        private const string MdbResourceName = "EmptyDatabase.mdb";

        private static string _msAccessAceProvider = null;
        private static string _msAccessJetProvider = null;

        /// <summary>
        /// Creates an empty Microsoft Access database at location <paramref name="path"/>. Database type
        /// (<c>.accdb</c>, <c>.mdb</c>) is specified with <paramref name="provider"/>.
        /// </summary>
        /// <param name="path">Path, where the database will be created. The path must be full, must contain also file name.
        /// </param>
        /// <param name="provider">Microsoft Access database type.</param>
        /// <remarks>
        /// <alert class="warning">
        /// <para>If the file <paramref name="path"/> already exists, it will be overwritten.</para>
        /// </alert>
        /// <para>
        /// Based on the value of <paramref name="provider"/> is created specified database type (<c>.accdb</c>, or older
        /// <c>.mdb</c>). But nothing is done with the file extension, so the file name will be created as
        /// <paramref name="path"/>. So it is possible to create a file with <c>.mdb</c> extension, which actually will be
        /// database type <c>.accdb</c>. So the caller must provide correct file name.
        /// </para>
        /// </remarks>
        public static void CreateEmptyDatabase(string path, ProviderType provider)
        {
            string resourceFileName = (provider == ProviderType.Ace ? AccdbResourceName : MdbResourceName);
            string resourceName = $"{ResourcesNamespace}.{resourceFileName}";

            using (Stream sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (FileStream writer = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                sourceStream.CopyTo(writer);
            }
        }

        /// <summary>
        /// Returns installed provider for Microsoft Access. ACE provider is preferred over JET provider (if both are available).
        /// If none provider is installed, empty string is returned.
        /// </summary>
        public static string MsAccessProvider => MsAccessAceProvider == string.Empty ? MsAccessJetProvider : MsAccessAceProvider;

        /// <summary>
        /// Returns is specified Microsoft Access provider <paramref name="provider"/> is available.
        /// </summary>
        /// <param name="provider">Checked provider type.</param>
        /// <returns><see langword="true"/> if provider is available, otherwise <see langword="false"/>.</returns>
        public static bool HasProvider(ProviderType provider) =>
            provider == ProviderType.Ace
                ? !string.IsNullOrEmpty(MsAccessAceProvider)
                : !string.IsNullOrEmpty(MsAccessJetProvider);

        /// <summary>
        /// For specified provider type <paramref name="provider"/> returns string of the provider for use in connection string.
        /// If there is no provider available, empty string is returned.
        /// </summary>
        /// <param name="provider">Provider type.</param>
        /// <returns>
        /// Method just returns the values of other properties:
        /// <list type="table">
        /// <item>
        ///   <term><see cref="ProviderType.Ace">ProviderType.Ace</see></term>
        ///   <description><see cref="MsAccessAceProvider"/></description>
        /// </item>
        /// <item>
        ///   <term><see cref="ProviderType.Jet">ProviderType.Jet</see></term>
        ///   <description><see cref="MsAccessJetProvider"/></description>
        /// </item>
        /// </list>
        /// </returns>
        public static string GetProviderString(ProviderType provider)
        {
            return provider == ProviderType.Jet ? MsAccessJetProvider : MsAccessAceProvider;
        }

        /// <summary>
        /// Returns string for installed Microsoft Access ACE provider (for example <b>Microsoft.ACE.OLEDB.12.0</b>).
        /// If ACE provider is not installed, empty string is returned.
        /// </summary>
        /// <remarks>
        /// Provider is loaded from the system only once and the value is cached. So when no provider is found, this state is
        /// returned for any subsequent reads of the property.
        /// </remarks>
        public static string MsAccessAceProvider
        {
            get
            {
                if (_msAccessAceProvider == null)
                {
                    _msAccessAceProvider = GetMsAccessProvider(AceProviderBase);
                }
                return _msAccessAceProvider;
            }
        }

        /// <summary>
        /// Returns string for installed Microsoft Access ACE provider (for example <b>Microsoft.Jet.OLEDB.4.0</b>).
        /// If JET provider is not installed, empty string is returned.
        /// </summary>
        /// <remarks>
        /// Provider is loaded from the system only once and the value is cached. So when no provider is found, this state is
        /// returned for any subsequent reads of the property.
        /// </remarks>
        public static string MsAccessJetProvider
        {
            get
            {
                if (_msAccessJetProvider == null)
                {
                    _msAccessJetProvider = GetMsAccessProvider(JetProviderBase);
                }
                return _msAccessJetProvider;
            }
        }

        private static string GetMsAccessProvider(string providerBase)
        {
            using (OleDbDataReader reader = OleDbEnumerator.GetRootEnumerator())
            {
                while (reader.Read())
                {
                    string providerName = reader.GetString(reader.GetOrdinal("SOURCES_NAME")).Trim();
                    if (providerName.StartsWith(providerBase, StringComparison.OrdinalIgnoreCase))
                    {
                        return providerName;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns Microsoft Access provider type used in connection <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Provider type.</returns>
        public static ProviderType GetProviderType(IDbConnection connection)
        {
            return GetProviderType(connection.ConnectionString);
        }

        /// <summary>
        /// Returns Microsoft Access provider type used in connection string <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns>Provider type.</returns>
        public static ProviderType GetProviderType(string connectionString)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            if (IsMsAccessAceProvider(connectionString))
            {
                return ProviderType.Ace;
            }
            else if (IsMsAccessJetProvider(connectionString))
            {
                return ProviderType.Jet;
            }
            throw new InvalidOperationException(Resources.ConnectionStringIsNotMsAccess);
        }

        /// <summary>
        /// Checks if database connection <paramref name="connectionString"/> is Microsoft Access.
        /// </summary>
        /// <param name="connectionString">Tested connection string.</param>
        /// <returns><see langword="true"/> if connection is to Microsoft Access database, otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsMsAccessConnection(string connectionString) =>
            IsMsAccessAceProvider(connectionString) || IsMsAccessJetProvider(connectionString);

        /// <summary>
        /// Checks if database connection <paramref name="connection"/> is Microsoft Access.
        /// </summary>
        /// <param name="connection">Tested connection.</param>
        /// <returns><see langword="true"/> if connection is to Microsoft Access database, otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsMsAccessConnection(IDbConnection connection)
        {
            return IsMsAccessConnection(connection.ConnectionString);
        }

        /// <summary>
        /// Checks if ACE provider is used in <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns><see langword="true"/> if ACE provider is used, otherwise <see langword="false"/>.</returns>
        public static bool IsMsAccessAceProvider(string connectionString) =>
            GetProviderName(connectionString).StartsWith(AceProviderBase, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks if JET provider is used in <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns><see langword="true"/> if JET provider is used, otherwise <see langword="false"/>.</returns>
        public static bool IsMsAccessJetProvider(string connectionString) =>
            GetProviderName(connectionString).StartsWith(JetProviderBase, StringComparison.OrdinalIgnoreCase);

        private static string GetProviderName(string connectionString)
        {
            string ret = string.Empty;
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connectionString);

            if (builder.Provider != null)
            {
                ret = builder.Provider.Trim();
            }

            return ret;
        }

        /// <summary>
        /// Checks if connection <paramref name="connection"/> is an exclusive connection to the database.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns><see langword="true"/> if connection is exclusive, otherwise <see langword="false"/>.</returns>
        public static bool IsExclusiveMsAccessConnection(IDbConnection connection)
        {
            if (connection.GetType() == typeof(OleDbConnection))
            {
                return IsExclusiveMsAccessConnection(connection.ConnectionString);
            }
            return false;
        }

        /// <summary>
        /// Checks if connection <paramref name="connectionString"/> is an exclusive connection to the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns><see langword="true"/> if connection is exclusive, otherwise <see langword="false"/>.</returns>
        public static bool IsExclusiveMsAccessConnection(string connectionString)
        {
            return
                ((connectionString.IndexOf("Share Deny Read", StringComparison.OrdinalIgnoreCase) >= 0) &&
                 (connectionString.IndexOf("Share Deny Write", StringComparison.OrdinalIgnoreCase) >= 0)) ||
                (connectionString.IndexOf("Share Exclusive", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Creates a connection string to the database <paramref name="databasePath"/> with provider type
        /// <paramref name="provider"/>.
        /// </summary>
        /// <param name="databasePath">Path to the database.</param>
        /// <param name="provider">Provider type, which will be usedProvider string is used for the current, installed provider,
        /// so there is no need to care about provider version.</param>
        /// <returns>Returns connection string to specified database. For example
        /// <c>Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\data\database.accdb;</c>.</returns>
        public static string CreateConnectionString(string databasePath, ProviderType provider)
            => string.Format(BaseConnectionString, GetProviderString(provider), databasePath);
    }
}
