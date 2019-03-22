using System;

namespace Kros.Data
{
    /// <summary>
    /// Interface for generating IDs for records in database. In general, IDs are just sequential numbers.
    /// </summary>
    /// <remarks>Usually one generator generates IDs for just one table.</remarks>
    /// <seealso cref="SqlServer.SqlServerIdGenerator"/>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public interface IIdGenerator : IDisposable
    {
        /// <summary>
        /// Returns next ID.
        /// </summary>
        /// <returns>
        /// Unique ID for record in data table.
        /// </returns>
        int GetNext();

        /// <summary>
        /// Initializes database for using ID generator. Initialization can mean creating necessary table and stored procedure.
        /// </summary>
        void InitDatabaseForIdGenerator();
    }
}
