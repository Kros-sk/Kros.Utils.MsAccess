using System;

namespace Kros.Extensions
{
    /// <summary>
    /// Extension methods for date and time <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns date, which is the first day in month and year in input <paramref name="value"/>. Time component is nulled.
        /// </summary>
        /// <param name="value">Date, to which the first day of month is returned.</param>
        /// <returns>Date.</returns>
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// Returns first day of current month.
        /// </summary>
        /// <returns>Date.</returns>
        public static DateTime FirstDayOfCurrentMonth()
        {
            return DateTime.Now.FirstDayOfMonth();
        }

        /// <summary>
        /// Returns date, which is the last day in month and year in input <paramref name="value"/>. Time component is nulled.
        /// </summary>
        /// <param name="value">Date, to which the last day of month is returned.</param>
        /// <returns>Date.</returns>
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
        }

        /// <summary>
        /// Returns last day of current month.
        /// </summary>
        /// <returns>Date.</returns>
        public static DateTime LastDayOfCurrentMonth()
        {
            return DateTime.Now.LastDayOfMonth();
        }
    }
}
