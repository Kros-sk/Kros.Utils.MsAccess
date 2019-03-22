using Kros.Data;
using Kros.Data.BulkActions;
using Kros.Data.BulkActions.MsAccess;
using Kros.Data.MsAccess;
using Kros.Data.Schema;
using Kros.Data.Schema.MsAccess;

namespace Kros.Utils.MsAccess
{
    /// <summary>
    /// Initialization of the library.
    /// </summary>
    public static class LibraryInitializer
    {
        /// <summary>
        /// Initializes the library. Method should be called once at a start of a program.
        /// </summary>
        /// <remarks>
        /// The initialization will do:
        /// <list type="bullet">
        /// <item>Adds <see cref="MsAccessSchemaLoader"/> to the default static list
        /// <see cref="DatabaseSchemaLoader.Default">DatabaseSchemaLoader.Default</see>.</item>
        /// <item>Adds <see cref="MsAccessSchemaLoader"/> to the defaule schema cache
        /// <see cref="DatabaseSchemaCache.Default">DatabaseSchemaCache.Default</see>.</item>
        /// <item>Registers <see cref="MsAccessIdGeneratorFactory"/> to the <see cref="IdGeneratorFactories"/>.</item>
        /// <item>Registers <see cref="MsAccessBulkActionFactory"/> to the <see cref="BulkActionFactories"/>.</item>
        /// </list>
        /// </remarks>
        public static void InitLibrary()
        {
            DatabaseSchemaLoader.Default.AddSchemaLoader(new MsAccessSchemaLoader());
            DatabaseSchemaCache.Default.AddSchemaLoader(new MsAccessSchemaLoader(), new MsAccessCacheKeyGenerator());
            MsAccessIdGeneratorFactory.Register();
            MsAccessBulkActionFactory.Register();
        }
    }
}
