namespace System
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeEx
    {
        /// <summary>
        /// Gets a <see cref="Date"/> value that represents the date component of the current
        /// <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> instance.</param>
        /// <returns>The <see cref="Date"/> value.</returns>
        public static Date Date(this DateTime dateTime)
        {
            return new Date((int) (dateTime.Ticks / TimeSpan.TicksPerDay));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> value that represents the time component of the current
        /// <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> instance.</param>
        /// <returns>The <see cref="TimeOfDay"/> value.</returns>
        public static TimeOfDay TimeOfDay(this DateTime dateTime)
        {
            return new TimeOfDay(dateTime.TimeOfDay.Ticks);
        }
        
        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to the current date and time in the specified time zone.
        /// </summary>
        /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/> instance.</param>
        /// <returns>The current <see cref="DateTime"/> for the specified time zone.</returns>
        public static DateTime NowInTimeZone(TimeZoneInfo timeZoneInfo)
        {
            // TODO: Propose placing this method directly in the System.DateTime struct
            
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo).DateTime;
        }
    }
}
