using Kros.Utils;
using System;
using System.Data;

namespace Kros.Data
{
    /// <summary>
    /// Helper class for opening database connection. It ensures opening and closing of the connection.
    /// </summary>
    public class ConnectionHelper
        : Suspender
    {
        /// <summary>
        /// If database <paramref name="connection"/> is closed, it is opened immediately. After disposing of returned
        /// object, the connection is closed, but only if it was opened. So when already opened connection is passed
        /// in the parameter, nothing is done with it.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>
        /// Helper object, which closes connection when it is disposed of.
        /// </returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public static IDisposable OpenConnection(IDbConnection connection)
        {
            Check.NotNull(connection, nameof(connection));
            var helper = new ConnectionHelper(connection);
            return helper.Suspend();
        }

        private readonly IDbConnection _connection;
        private bool _closeConnection = false;

        private ConnectionHelper(IDbConnection connection)
        {
            _connection = connection;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void SuspendCore()
        {
            base.SuspendCore();
            if (!_connection.IsOpened())
            {
                _closeConnection = true;
                _connection.Open();
            }
        }

        protected override void ResumeCore()
        {
            base.ResumeCore();
            if (_closeConnection)
            {
                _connection.Close();
                _closeConnection = false;
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
