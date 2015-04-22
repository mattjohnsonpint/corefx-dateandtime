namespace System
{
    public static class TimeZoneInfoEx
    {
        public static DateTimeOffset GetCurrentDateTimeOffset(this TimeZoneInfo timeZoneInfo)
        {
            return DateTimeOffsetEx.NowInTimeZone(timeZoneInfo);
        }

        public static DateTime GetCurrentDateTime(this TimeZoneInfo timeZoneInfo)
        {
            return DateTimeEx.NowInTimeZone(timeZoneInfo);
        }
        
        public static Date GetCurrentDate(this TimeZoneInfo timeZoneInfo)
        {
            return Date.TodayInTimeZone(timeZoneInfo);
        }

        public static TimeOfDay GetCurrentTime(this TimeZoneInfo timeZoneInfo)
        {
            return TimeOfDay.NowInTimeZone(timeZoneInfo);
        }
    }
}
