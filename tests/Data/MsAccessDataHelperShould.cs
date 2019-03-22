using FluentAssertions;
using Kros.Data.MsAccess;
using Kros.Utils.UnitTests;
using System;
using System.Data.OleDb;
using System.IO;
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

        [SkippableFact]
        public void CreateEmptyAccdbDatabase()
        {
            Helpers.SkipTestIfAceProviderNotAvailable();
            CreateEmptyDatabaseCore(ProviderType.Ace);
        }

        [SkippableFact]
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
                connectToEmptyDbAction.Should().NotThrow();
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
            ((Action)(() => MsAccessDataHelper.GetProviderType(connectionString))).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetProviderType_EmptyConnectionString()
        {
            ((Action)(() => MsAccessDataHelper.GetProviderType(string.Empty))).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetProviderType_BaseConnectionString()
        {
            var connectionString = string.Format(BaseConnectionString, string.Empty);
            ((Action)(() => MsAccessDataHelper.GetProviderType(connectionString)))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetProviderType_AceConnectionString()
        {
            MsAccessDataHelper.GetProviderType(string.Format(BaseConnectionString, AceProvider))
                .Should().Be(ProviderType.Ace);
        }

        [Fact]
        public void GetProviderType_JetConnectionString()
        {
            MsAccessDataHelper.GetProviderType(string.Format(BaseConnectionString, JetProvider))
                .Should().Be(ProviderType.Jet);
        }

        [Fact]
        public void GetProviderType_SqlConnectionString()
        {
            var connectionString = string.Format(BaseConnectionString, SqlProvider);
            ((Action)(() => MsAccessDataHelper.GetProviderType(connectionString)))
                .Should().Throw<InvalidOperationException>();
        }

        #endregion

        #region Is MsAccess Ace Provider

        [Fact]
        public void IsMsAccesAceProvider_EmptyConnectionString() =>
            MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, string.Empty)).Should().BeFalse();

        [Fact]
        public void IsMsAccesAceProvider_AceConnectionString() =>
            MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, AceProvider)).Should().BeTrue();

        [Fact]
        public void IsMsAccesAceProvider_JetConnectionString() =>
            MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, JetProvider)).Should().BeFalse();

        [Fact]
        public void IsMsAccesAceProvider_SqlConnectionString() =>
            MsAccessDataHelper.IsMsAccessAceProvider(string.Format(BaseConnectionString, SqlProvider)).Should().BeFalse();

        #endregion

        #region Is MsAccess Jet Provider

        [Fact]
        public void IsMsAccesJetProvider_EmptyConnectionString() =>
            MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, string.Empty)).Should().BeFalse();

        [Fact]
        public void IsMsAccesJetProvider_AceConnectionString() =>
            MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, AceProvider)).Should().BeFalse();

        [Fact]
        public void IsMsAccesJetProvider_JetConnectionString() =>
            MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, JetProvider)).Should().BeTrue();

        [Fact]
        public void IsMsAccesJetProvider_SqlConnectionString() =>
            MsAccessDataHelper.IsMsAccessJetProvider(string.Format(BaseConnectionString, SqlProvider)).Should().BeFalse();

        #endregion
    }
}
