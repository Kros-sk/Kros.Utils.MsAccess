using Kros.Data.Schema;
using System.Data.SqlClient;

namespace Kros.Examples
{
    internal class SchemaExamples
    {
        public void LoadSchemaExample()
        {
            #region SchemaLoader
            SqlConnection cn = new SqlConnection("...");

            DatabaseSchema schema = DatabaseSchemaLoader.Default.LoadSchema(cn);
            #endregion
        }

        public void SchemaCacheExample()
        {
            #region SchemaCache
            SqlConnection cn = new SqlConnection("...");

            // Použitie vytvorením si vlastnej inštancie.
            var cache = new DatabaseSchemaCache();

            DatabaseSchema schema = cache.GetSchema(cn);

            // Použitie statickej vlastnosti.
            schema = DatabaseSchemaCache.Default.GetSchema(cn);
            #endregion
        }
    }
}
