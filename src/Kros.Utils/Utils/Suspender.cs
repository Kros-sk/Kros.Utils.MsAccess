using System;

namespace Kros.Utils
{
    /// <summary>
    /// Class for simple work suspending. It is poosible to suspend work (<see cref="Suspend"/>) several times, but in that
    /// case it is necessary to resume (<see cref="Resume"/>) it the same number of times. The easiest way of suspending
    /// work is using the <c>using</c> block.
    /// </summary>
    /// <remarks>
    /// It is useful for example in object initialization. During the initialization it is often necessary not to perform
    /// certain actions. Standard way is using some flag if the initialization is running. The <c>Suspender</c> class
    /// encapsulates the management of this flag, while it is possible to set this flag several times in succession
    /// (nested work suspending).
    /// </remarks>
    /// <example>
    /// <code language = "cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\SuspenderExamples.cs" region="Init" />
    /// </example>
    public class Suspender
    {
        #region Nested typed

        private class SuspenderInternal : IDisposable
        {
            private Suspender _suspender;

            public SuspenderInternal(Suspender suspender)
            {
                _suspender = suspender;
            }

            bool _disposed = false;

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    _suspender.Resume();
                }
                _disposed = true;
            }
        }

        #endregion

        private int _counter = 0;

        /// <summary>
        /// Suspends the - sets the <see cref="IsSuspended"/> flag to <see langword="true"/>. If this method is called several
        /// times, it is necessary to call <see cref="Resume"/> the same number of times to clear <see cref="IsSuspended"/> flag.
        /// </summary>
        /// <returns>
        /// Returns helper object, which automatically calls <see cref="Resume"/> when disposed of, so it is convenient
        /// to use <c>using</c> block.
        /// </returns>
        /// <example>
        /// <code language = "cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\SuspenderExamples.cs" region="Init" />
        /// </example>
        public IDisposable Suspend()
        {
            if (_counter == 0)
            {
                SuspendCore();
            }
            _counter++;
            return new SuspenderInternal(this);
        }

        /// <summary>
        /// Used for specific (inherited) suspender implementations. This method is executed when <see cref="Suspend"/> is called
        /// for the first time. So it means it is executed when <see cref="IsSuspended"/> flag is changing from
        /// <see langword="false"/> to <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// Method is intended for implementing custom logic in own suspender when suspending work. It is executed only
        /// once during the first call of <see cref="Suspend"/> (subsequent calls to <see cref="Suspend"/> do not call
        /// <see cref="SuspendCore"/>). The method is called <b>before</b> the <see cref="IsSuspended"/> flag is changed,
        /// so the value of flag while the method is executing is <see langword="false"/>.
        /// </remarks>
        protected virtual void SuspendCore()
        {
        }

        private void Resume()
        {
            _counter--;
            if (_counter == 0)
            {
                ResumeCore();
            }
        }

        /// <summary>
        /// Used for specific (inherited) suspender implementations. This method is executed when the work is resumed
        /// for the last time. So it means it is executed when <see cref="IsSuspended"/> flag is changing from
        /// <see langword="true"/> to <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// Method is intended for implementing custom logic in own suspender when resuming work. It is executed only
        /// once during the last work resuming (preceding work resumings do not call <see cref="ResumeCore"/>).
        /// The method is called <b>after</b> the <see cref="IsSuspended"/> flag is changed,
        /// so the value of flag while the method is executing is <see langword="false"/>.
        /// </remarks>
        protected virtual void ResumeCore()
        {
        }

        /// <summary>
        /// The flag, if work is (<see langword="true"/>), or not (<see langword="false"/>).
        /// </summary>
        public bool IsSuspended => _counter > 0;
    }
}