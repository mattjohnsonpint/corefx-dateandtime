namespace System
{
    /// <summary>
    /// Extension methods for <see cref="System.DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetEx
    {
        public static Date Date(this DateTimeOffset dateTimeOffset)
        {
            return new Date((int)(dateTimeOffset.DateTime.Ticks / TimeSpan.TicksPerDay));
        }

        public static TimeOfDay TimeOfDay(this DateTimeOffset dateTimeOffset)
        {
            return new TimeOfDay(dateTimeOffset.TimeOfDay.Ticks);
        }

        // TODO: Propose placing this method directly in the System.DateTimeOffset struct
        public static DateTimeOffset NowInTimeZone(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return TimeZoneInfo.ConvertTime(utcNow, timeZone);
        }
    }
}
