using System;

namespace Kros.Utils
{
    /// <summary>
    /// Class for "freezing" date and time to constant value. Usable for example in tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Current time is accessible in <see cref="Now"/> property. Own time can be injected using
    /// <see cref="InjectActualDateTime(DateTime)"/>.
    /// </para>
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\DateTimeProviderExamples.cs" region="BasicExample"/>
    /// <para>
    /// Set value is valid for current thread only, so it is possible to have different values in different threads.
    /// </para>
    /// </remarks>
    /// <seealso cref="System.IDisposable" />
    public class DateTimeProvider : IDisposable
    {
        [ThreadStatic]
        private static DateTime? _injectedDateTime;

        private DateTimeProvider()
        {
        }

        /// <summary>
        /// Returns own date and time, if it was set by <see cref="InjectActualDateTime(DateTime)"/>. If it was not set,
        /// <see cref="DateTime.Now">DateTime.Now</see> is returned.
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return _injectedDateTime ?? DateTime.Now;
            }
        }

        /// <summary>
        /// Sets time <paramref name="actualDateTime"/>, which will be returned in <see cref="Now"/> property.
        /// Use it in <c>using</c> block.
        /// </summary>
        /// <param name="actualDateTime">Required date and time value.</param>
        public static IDisposable InjectActualDateTime(DateTime actualDateTime)
        {
            _injectedDateTime = actualDateTime;

            return new DateTimeProvider();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Dispose()
        {
            _injectedDateTime = null;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
