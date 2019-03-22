using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Kros.Data.BulkActions
{
    /// <summary>
    /// Representing set of registered <see cref="Kros.Data.BulkActions.IBulkActionFactory"/>.
    /// </summary>
    public static class BulkActionFactories
    {
        private static Dictionary<string, Func<string, IBulkActionFactory>> _factoryByAdoClientName =
            new Dictionary<string, Func<string, IBulkActionFactory>>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<Type, Func<DbConnection, IBulkActionFactory>> _factoryByConnection =
            new Dictionary<Type, Func<DbConnection, IBulkActionFactory>>();

        static BulkActionFactories()
        {
            SqlServer.SqlServerBulkActionFactory.Register();
        }

        /// <summary>
        /// Registers the specified ADO client name.
        /// </summary>
        /// <typeparam name="TConnection">The type of the connection.</typeparam>
        /// <param name="adoClientName">Name of the database client.</param>
        /// <param name="factoryByConnection">The factory by connection.</param>
        /// <param name="factoryByConnectionString">The factory by connection string.</param>
        public static void Register<TConnection>(string adoClientName,
            Func<DbConnection, IBulkActionFactory> factoryByConnection,
            Func<string, IBulkActionFactory> factoryByConnectionString)
            where TConnection : DbConnection
        {
            Check.NotNullOrWhiteSpace(adoClientName, nameof(adoClientName));

            _factoryByAdoClientName[adoClientName] = Check.NotNull(factoryByConnectionString, nameof(factoryByConnectionString));
            _factoryByConnection[typeof(TConnection)] = Check.NotNull(factoryByConnection, nameof(factoryByConnection));
        }

        /// <summary>
        /// Gets the <see cref="IBulkActionFactory"/> with specific connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>
        /// The <see cref="IBulkActionFactory"/> instance.
        /// </returns>
        public static IBulkActionFactory GetFactory(DbConnection connection)
        {
            if (_factoryByConnection.TryGetValue(connection.GetType(), out var factory))
            {
                return factory(connection);
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForConnection,
                    nameof(IBulkActionFactory), connection.GetType().FullName));
            }
        }

        /// <summary>
        /// Gets the <see cref="IBulkActionFactory"/> with specific connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="adoClientName">
        /// Name of the ado client. (e.g. <see cref="System.Data.SqlClient.SqlConnection"/> it's: System.Data.SqlClient)
        /// </param>
        /// <returns>
        /// The <see cref="IBulkActionFactory"/> instance.
        /// </returns>
        public static IBulkActionFactory GetFactory(string connectionString, string adoClientName)
        {
            if (_factoryByAdoClientName.TryGetValue(adoClientName, out var factory))
            {
                return factory(connectionString);
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForClient,
                    nameof(IBulkActionFactory), adoClientName));
            }
        }
    }
}
