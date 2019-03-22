using Kros.Properties;
using Kros.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Kros.Data
{
    /// <summary>
    /// Helper class for ID generator factories (<see cref="IIdGeneratorFactory"/>) for different databases.
    /// Factories are registered in the class using
    /// <see cref="IdGeneratorFactories.Register{TConnection}(string, Func{DbConnection, IIdGeneratorFactory}, Func{string, IIdGeneratorFactory})"/>
    /// method. Two factory methods are registered for every connection (database) type. One for creating generator
    /// with connection instance and one with connection string.
    /// </summary>
    /// <remarks>
    /// <see cref="SqlServer.SqlServerIdGeneratorFactory"/> is automatically registered.
    /// </remarks>
    /// <seealso cref="IIdGeneratorFactory"/>
    /// <seealso cref="IIdGenerator"/>
    public static class IdGeneratorFactories
    {
        private static Dictionary<string, Func<string, IIdGeneratorFactory>> _factoryByAdoClientName =
            new Dictionary<string, Func<string, IIdGeneratorFactory>>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<Type, Func<DbConnection, IIdGeneratorFactory>> _factoryByConnection =
            new Dictionary<Type, Func<DbConnection, IIdGeneratorFactory>>();

        static IdGeneratorFactories()
        {
            SqlServer.SqlServerIdGeneratorFactory.Register();
        }

        /// <summary>
        /// Registers ID generator factory methods <paramref name="factoryByConnection"/> and
        /// <paramref name="factoryByConnectionString"/> for database specified by connection type
        /// <typeparamref name="TConnection"/> and client name <paramref name="adoClientName"/>.
        /// </summary>
        /// <typeparam name="TConnection">Database connection type.</typeparam>
        /// <param name="adoClientName">
        /// Name of the database client. It identifies specific database. For example client name for
        /// <see cref="SqlServer.SqlServerIdGeneratorFactory"/> is "System.Data.SqlClient"
        /// (<see cref="SqlServer.SqlServerDataHelper.ClientId"/>).
        /// </param>
        /// <param name="factoryByConnection">
        /// Factory method for creating <see cref="IIdGeneratorFactory"/> with connection instance.
        /// </param>
        /// <param name="factoryByConnectionString">
        /// Factory method for creating <see cref="IIdGeneratorFactory"/> with connection string.
        /// </param>
        /// <exception cref="ArgumentNullException">Value of any argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Value of <paramref name="adoClientName"/> is empty string, or string
        /// containing only whitespace characters.</exception>
        public static void Register<TConnection>(
            string adoClientName,
            Func<DbConnection, IIdGeneratorFactory> factoryByConnection,
            Func<string, IIdGeneratorFactory> factoryByConnectionString)
            where TConnection : DbConnection
        {
            Check.NotNullOrWhiteSpace(adoClientName, nameof(adoClientName));

            _factoryByAdoClientName[adoClientName] = Check.NotNull(factoryByConnectionString, nameof(factoryByConnectionString));
            _factoryByConnection[typeof(TConnection)] = Check.NotNull(factoryByConnection, nameof(factoryByConnection));
        }

        /// <summary>
        /// Returns <see cref="IIdGeneratorFactory"/> for specified <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>Instance of <see cref="IIdGeneratorFactory"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Factory for type of <paramref name="connection"/> is not registered.
        /// </exception>
        public static IIdGeneratorFactory GetFactory(DbConnection connection)
        {
            if (_factoryByConnection.TryGetValue(connection.GetType(), out var factory))
            {
                return factory(connection);
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForConnection,
                    nameof(IIdGeneratorFactory), connection.GetType().FullName));
            }
        }

        /// <summary>
        /// Returns <see cref="IIdGeneratorFactory"/> for specified <paramref name="connectionString"/> and database
        /// type <paramref name="adoClientName"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for database.</param>
        /// <param name="adoClientName">Name, which specifies database type.</param>
        /// <returns>Instance of <see cref="IIdGeneratorFactory"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Factory for database type specified by <paramref name="adoClientName"/> is not registered.
        /// </exception>
        public static IIdGeneratorFactory GetFactory(string connectionString, string adoClientName)
        {
            if (_factoryByAdoClientName.TryGetValue(adoClientName, out var factory))
            {
                return factory(connectionString);
            }
            else
            {
                throw new InvalidOperationException(string.Format(Resources.FactoryNotRegisteredForClient,
                    nameof(IIdGeneratorFactory), adoClientName));
            }
        }
    }
}
