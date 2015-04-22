﻿namespace System
{
    /// <summary>
    /// Extension methods for <see cref="System.DateTime"/>.
    /// </summary>
    public static class DateTimeEx
    {
        public static Date Date(this DateTime dateTime)
        {
            return new Date((int) (dateTime.Ticks / TimeSpan.TicksPerDay));
        }

        public static TimeOfDay TimeOfDay(this DateTime dateTime)
        {
            return new TimeOfDay(dateTime.TimeOfDay.Ticks);
        }
        
        // TODO: Propose placing this method directly in the System.DateTime struct
        public static DateTime NowInTimeZone(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            return TimeZoneInfo.ConvertTime(utcNow, timeZone).DateTime;
        }

        public static DateTime NowLocal()
        {
            return DateTime.Now;
        }

        public static DateTime NowUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
