namespace System
{
    /// <summary>
    /// Extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetEx
    {
        /// <summary>
        /// Gets a <see cref="Date"/> value that represents the date component of the current
        /// <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> instance.</param>
        /// <returns>The <see cref="Date"/> value.</returns>
        public static Date Date(this DateTimeOffset dateTimeOffset)
        {
            return new Date((int)(dateTimeOffset.DateTime.Ticks / TimeSpan.TicksPerDay));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> value that represents the time component of the current
        /// <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> instance.</param>
        /// <returns>The <see cref="TimeOfDay"/> value.</returns>
        public static TimeOfDay TimeOfDay(this DateTimeOffset dateTimeOffset)
        {
            return new TimeOfDay(dateTimeOffset.TimeOfDay.Ticks);
        }

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object that is set to the current date, time,
        /// and offset from Coordinated Universal Time (UTC) in the specified time zone.
        /// </summary>
        /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/> instance.</param>
        /// <returns>The current <see cref="DateTimeOffset"/> for the specified time zone.</returns>
        public static DateTimeOffset NowInTimeZone(TimeZoneInfo timeZoneInfo)
        {
            // TODO: Propose placing this method directly in the System.DateTimeOffset struct

            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo);
        }
    }
}
