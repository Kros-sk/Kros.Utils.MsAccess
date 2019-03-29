using Kros.Utils;

namespace Kros.Examples
{
    class SuspenderExamples
    {
        #region Init
        private Suspender _initSuspender = new Suspender();

        private void Init()
        {
            using (_initSuspender.Suspend())
            {
                // Do initialization...
            }
        }

        private void DoWork()
        {
            // Do general work.

            if (!_initSuspender.IsSuspended)
            {
                // Do work only when not initializing.
            }
        }
        #endregion
    }
}
