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



        public static DateTimeOffset Now(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return TimeZoneInfo.ConvertTime(utcNow, timeZone);
        }

        public static DateTimeOffset NowLocal()
        {
            return DateTimeOffset.Now;
        }

        public static DateTimeOffset NowUtc()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
