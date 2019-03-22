using Kros.Data.Schema.MsAccess;
using Kros.MsAccess.Properties;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Reflection;

namespace Kros.Data.MsAccess
{
    /// <summary>
    /// The unique ID generator for Microsoft Access.
    /// </summary>
    /// <seealso cref="IdGeneratorFactories" />
    /// <seealso cref="MsAccessIdGeneratorFactory" />
    /// <remarks>In general, the generator should be created using <see cref="MsAccessIdGeneratorFactory"/>.</remarks>
    /// <example>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public class MsAccessIdGenerator : IdGeneratorBase
    {
        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connectionString"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connectionString">Connection string to the database.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connectionString"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="connectionString"/> is empty string, or string containing only
        /// whitespace characters.</item>
        /// <item>Value of <paramref name="batchSize"/> is less or equal than 0.</item>
        /// </list></exception>
        public MsAccessIdGenerator(string connectionString, string tableName, int batchSize)
            : base(connectionString, tableName, batchSize)
        {
        }

        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connection"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connection"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Value of <paramref name="batchSize"/> is less or equal than 0.</exception>
        public MsAccessIdGenerator(OleDbConnection connection, string tableName, int batchSize)
            : base(connection, tableName, batchSize)
        {
        }

        /// <inheritdoc/>
        protected override DbConnection CreateConnection(string connectionString) =>
            new OleDbConnection(connectionString);

        /// <inheritdoc/>
        protected override int GetNewIdFromDbCore()
        {
            int result = 0;

            var valueIsOk = GetNewIDMsAccessCore(Connection as OleDbConnection, TableName, BatchSize, ref result);
            while (!valueIsOk)
            {
                valueIsOk = GetNewIDMsAccessCore(Connection as OleDbConnection, TableName, BatchSize, ref result);
            }

            return result;
        }

        /// <summary>
        /// Returns SQL script for creating <c>IdStore</c> table.
        /// </summary>
        public static string GetIdStoreTableCreationScript() => Resources.SqlIdGeneratorTableScript;

        /// <inheritdoc/>
        public override void InitDatabaseForIdGenerator()
        {
            using (ConnectionHelper.OpenConnection(Connection))
            {
                var schemaLoader = new MsAccessSchemaLoader();
                if (schemaLoader.LoadTableSchema((OleDbConnection)Connection, "IdStore") == null)
                {
                    using (var cmd = Connection.CreateCommand())
                    {
                        string[] sqlCommands = GetIdStoreTableCreationScript()
                            .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string sqlCommand in sqlCommands)
                        {
                            string query = sqlCommand.Trim();
                            if (!string.IsNullOrEmpty(query))
                            {
                                cmd.CommandText = query;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        #region Private helpers

        private bool GetNewIDMsAccessCore(
            OleDbConnection cn,
            string tableName,
            int numberOfItems,
            ref int result)
        {
            DbTransaction existTransaction = GetExistingTransactionFromMsAccessConnection(cn);

            if (existTransaction == null)
            {
                return GetNewIDMsAccessWithNewTransaction(cn, tableName, numberOfItems, ref result);
            }
            else
            {
                return GetNewIDMsAccessWithExistTransaction(cn, tableName, numberOfItems, ref result, existTransaction);
            }
        }

        private DbTransaction GetExistingTransactionFromMsAccessConnection(OleDbConnection cn)
        {
            PropertyInfo pinfo = typeof(OleDbConnection)
                .GetProperty("InnerConnection", BindingFlags.Instance | BindingFlags.NonPublic);
            var innerConnection = pinfo.GetValue(cn, null);
            pinfo = innerConnection.GetType()
                .GetProperty("LocalTransaction", BindingFlags.Instance | BindingFlags.NonPublic);
            return pinfo == null ? null : (DbTransaction)pinfo.GetValue(innerConnection, null);
        }

        private bool GetNewIDMsAccessWithExistTransaction(
            OleDbConnection cn,
            string tableName,
            int numberOfItems,
            ref int result,
            DbTransaction transaction)
        {
            var valueIsOk = false;
            var actualValue = GetLastID(cn, transaction, tableName);
            var expectedValueInDB = (actualValue + numberOfItems); // Value expected in database after counter update.

            SaveChanges(cn, transaction, tableName, actualValue, numberOfItems);

            // If value in database is not as expected, somebody else was fast enough to update our counter,
            // so we need to start over.
            var actualValueInDB = GetLastID(cn, transaction, tableName);
            if (actualValueInDB == expectedValueInDB)
            {
                result = (actualValue + 1);
                valueIsOk = true;
            }

            return valueIsOk;
        }

        private bool GetNewIDMsAccessWithNewTransaction(
            OleDbConnection cn,
            string tableName,
            int numberOfItems,
            ref int result)
        {
            var valueIsOk = false;

            using (var transaction = (DbTransaction)cn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    valueIsOk = GetNewIDMsAccessWithExistTransaction(cn, tableName, numberOfItems, ref result, transaction);

                    if (valueIsOk)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        // This occurs when somebody else updated the value in the meantime.
                        // MS Access "waits" a while in transaction. If other transaction is fast enough,
                        // this one will not throw, but it will update already updated value.
                        transaction.Rollback();
                    }
                }
                catch (OleDbException ex) when (ex.MsAccessErrorCode() == MsAccessErrorCode.CouldNotUpdateCurrentlyLocked)
                {
                    // We can end here, if some other transaction updates counter, but not fast enougn.
                    // MS Access client throws exception, that data are locked.
                    transaction.Rollback();
                }
            }
            return valueIsOk;
        }

        private void SaveChanges(
            IDbConnection cn,
            DbTransaction transaction,
            string tableName,
            int actualValue,
            int numberOfItems)
        {
            if (actualValue == 0)
            {
                DeleteRecord(cn, transaction, tableName);
                InsertRecord(cn, transaction, tableName, numberOfItems);
            }
            else
            {
                IncrementLastId(cn, transaction, tableName, numberOfItems);
            }
        }

        private void DeleteRecord(
            IDbConnection cn,
            DbTransaction transaction,
            string tableName)
        {
            string query = "DELETE FROM IdStore WHERE TableName = @TableName";
            ExecuteNonQuery(cn, transaction, query, tableName);
        }

        private void InsertRecord(
            IDbConnection cn,
            DbTransaction transaction,
            string tableName,
            int lastID)
        {
            string query = "INSERT INTO IdStore (LastId, TableName) VALUES (@LastId, @TableName)";
            ExecuteNonQuery(cn, transaction, query, tableName, lastID);
        }

        private void IncrementLastId(
            IDbConnection cn,
            DbTransaction transaction,
            string tableName,
            int increment)
        {
            string query = "UPDATE IdStore SET LastID = LastID + @LastId WHERE (TableName = @TableName)";
            ExecuteNonQuery(cn, transaction, query, tableName, increment);
        }

        private void ExecuteNonQuery(
            IDbConnection cn,
            DbTransaction transaction,
            string query,
            string tableName,
            int numberOfItems)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = query;
                AddParameterWithValue(cmd, "@LastId", numberOfItems, DbType.Int32);
                AddParameterWithValue(cmd, "@TableName", tableName, DbType.String);
                cmd.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQuery(
            IDbConnection cn,
            DbTransaction transaction,
            string query,
            string tableName)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = query;
                AddParameterWithValue(cmd, "@TableName", tableName, DbType.String);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddParameterWithValue(IDbCommand cmd, string parameterName, object value, DbType dbType)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = dbType;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        private int GetLastID(IDbConnection cn, DbTransaction transaction, string tableName)
        {
            string queryAccess = "SELECT LastID FROM IdStore WHERE (TableName = @TableName)";

            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = queryAccess;
                AddParameterWithValue(cmd, "@TableName", tableName, DbType.String);
                var actualValue = cmd.ExecuteScalar();
                return Convert.ToInt32(actualValue);
            }
        }

        #endregion
    }
}