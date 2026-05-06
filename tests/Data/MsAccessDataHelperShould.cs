using Kros.Data.MsAccess;
using Kros.Utils.UnitTests;
using System.Data.OleDb;
using Xunit;

namespace Kros.Utils.MsAccess.UnitTests.Data
{
    /// <summary>
    /// Testy pre data helper.
    /// </summary>
    public class MsAccessDataHelperShould
    {
        #region Constants

        private const string AceProvider = "Microsoft.ACE.OLEDB.12.0";
        private const string JetProvider = "Microsoft.Jet.OLEDB.4.0";
        private const string SqlProvider = "SQLOLEDB";
        private const string BaseConnectionString = "Provider={0};Data Source=path;";

        #endregion

        #region General tests

        [Fact]
        public void CreateEmptyAccdbDatabase()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            CreateEmptyDatabaseCore(ProviderType.Ace);
        }

        [Fact]
        public void CreateEmptyMdbDatabase()
        {
            Helpers.SkipTestIfJetProviderNotAvailable();
            CreateEmptyDatabaseCore(ProviderType.Jet);
        }

        private void CreateEmptyDatabaseCore(ProviderType provider)
        {
            string emptyDbPath = Path.GetTempFileName();

            try
            {
                MsAccessDataHelper.CreateEmptyDatabase(emptyDbPath, provider);
                Action connectToEmptyDbAction = () =>
                {
                    using (var cn = new OleDbConnection(MsAccessDataHelper.CreateConnectionString(emptyDbPath, provider)))
                    {
                        cn.Open();
                    }
                };
                connectToEmptyDbAction();
            }
            finally
            {
                File.Delete(emptyDbPath);
            }
        }

        #endregion

        #region Get Provider Type

        [Fact]
        public void GetProviderType_NullConnectionString()
        {
            string connectionString = null;
            Assert.Throws<ArgumentNullException>(() => MsAccessDataHelper.GetProviderType(connectionString));
        }

        [Fact]
        public void GetProviderType_EmptyConnectionString()
        {
            Assert.Throws<ArgumentException>(() => MsAccessDataHelper.GetProviderType(string.Empty));
        }

        [Fact]
        public void GetProviderType_BaseConnectionString()
        {
            var connectionString = string.Format(BaseConnectionString, string.Empty);
            Assert.Throws<InvalidOperationException>(() => MsAccessDataHelper.GetProviderType(connectionString));
        }

        [Fact]
        public void GetProviderType_AceConnectionString()
        {
            Assert.Equal(ProviderType.Ace, MsAccessDataHelper.GetProviderType(string.Format(BaseConnectionString, AceProvider)));
        }

        [Fact]
        public void GetProviderType_JetConnectionString()
        {
            Assert.Equal(ProviderType.Jet, MsAccessDataHelper.GetProviderType(string.Format(BaseConnectionString, JetProvider)));
        }

        [Fact]
        public void GetProviderType_SqlConnectionString()
        {
            var connectionString = string.Format(BaseConnectionString, SqlProvider);
            Assert.Throws<InvalidOperationException>(() => MsAccessDataHelper.GetProviderType(connectionString));
        }

        #endregion

        #region Is MsAccess Ace Provider

        [Fact]
        public void IsMsAccesAceProvider_EmptyConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, string.Empty)));

        [Fact]
        public void IsMsAccesAceProvider_AceConnectionString()
            => Assert.True(MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, AceProvider)));

        [Fact]
        public void IsMsAccesAceProvider_JetConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, JetProvider)));

        [Fact]
        public void IsMsAccesAceProvider_SqlConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, SqlProvider)));

        #endregion

        #region Is MsAccess Jet Provider

        [Fact]
        public void IsMsAccesJetProvider_EmptyConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, string.Empty)));

        [Fact]
        public void IsMsAccesJetProvider_AceConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, AceProvider)));

        [Fact]
        public void IsMsAccesJetProvider_JetConnectionString()
            => Assert.True(MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, JetProvider)));

        [Fact]
        public void IsMsAccesJetProvider_SqlConnectionString()
            => Assert.False(MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, SqlProvider)));

        #endregion
    }
}
