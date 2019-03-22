using Kros.Data.MsAccess;
using Xunit;

namespace Kros.Utils.UnitTests
{
    internal static class Helpers
    {
        public const string RootNamespace = "Kros.Utils.MsAccess.UnitTests";
        public const string RootNamespaceResources = RootNamespace + ".Resources";

        private const string ProviderNotAvailableMessage = "MS Access provider {0} is not available.";

        public static void SkipTestIfAceProviderNotAvailable()
        {
            Skip.If(!MsAccessDataHelper.HasProvider(ProviderType.Ace),
                string.Format(ProviderNotAvailableMessage, MsAccessDataHelper.AceProviderBase));
        }

        public static void SkipTestIfJetProviderNotAvailable()
        {
            Skip.If(!MsAccessDataHelper.HasProvider(ProviderType.Jet),
                string.Format(ProviderNotAvailableMessage, MsAccessDataHelper.JetProviderBase));
        }
    }
}
