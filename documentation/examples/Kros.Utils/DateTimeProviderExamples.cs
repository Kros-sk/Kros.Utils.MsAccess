using System;

namespace Kros.Utils.Examples
{
    class DateTimeProviderExamples
    {
        #region BasicExample

        private void BasicExample()
        {
            using (DateTimeProvider.InjectActualDateTime(new DateTime(1978, 12, 10)))
            {
                Console.WriteLine(DateTimeProvider.Now.ToString("d.M.yyyy")); // Writes 10.12.1978
            }
        }

        #endregion
    }
}
