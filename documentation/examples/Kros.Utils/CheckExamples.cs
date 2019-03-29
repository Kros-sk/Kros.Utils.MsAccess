using Kros.Utils;
using System;

namespace Kros.Examples
{
    class CheckExamplesOld
    {
        #region CheckArgumentsOld

        private string _value1;
        private int _value2;

        public void MethodWithParameters(string arg1, int arg2)
        {
            if (string.IsNullOrEmpty(arg1))
            {
                throw new ArgumentNullException(nameof(arg1));
            }
            if (arg2 <= 0)
            {
                throw new ArgumentException("Hodnota parametra arg2 musí byť väčšia ako 0.", nameof(arg2));
            }

            _value1 = arg1;
            _value2 = arg2;

            // ...
        }

        #endregion
    }

    class CheckExamplesNew
    {
        #region CheckArgumentsNew

        private string _value1;
        private int _value2;

        public void MethodWithParameters(string arg1, int arg2)
        {
            _value1 = Check.NotNullOrEmpty(arg1, nameof(arg1));
            _value2 = Check.GreaterThan(arg2, 0, nameof(arg2));

            // ...
        }

        #endregion
    }
}
