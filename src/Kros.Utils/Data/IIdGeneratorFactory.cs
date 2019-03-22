namespace Kros.Data
{
    /// <summary>
    /// Interface for factory classes, which create instances of <see cref="IIdGenerator"/>.
    /// </summary>
    /// <seealso cref="SqlServer.SqlServerIdGeneratorFactory"/>
    /// <seealso cref="IdGeneratorFactories"/>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public interface IIdGeneratorFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IIdGenerator"/> for table <paramref name="tableName"/>.
        /// </summary>
        /// <param name="tableName">Table for which IDs will be generated.</param>
        /// <returns>The instance of <see cref="IIdGenerator"/>.</returns>
        IIdGenerator GetGenerator(string tableName);

        /// <summary>
        /// Creates an instance of <see cref="IIdGenerator"/> for table <paramref name="tableName"/>
        /// with specified <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="tableName">Table for which IDs will be generated.</param>
        /// <param name="batchSize">IDs batch size. This number of IDs will be reserved for later use.</param>
        /// <returns>The instance of <see cref="IIdGenerator"/>.</returns>
        IIdGenerator GetGenerator(string tableName, int batchSize);
    }
}
