﻿namespace System
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

        public static DateTimeOffset AddYears(this DateTimeOffset dateTimeOffset, int years, TimeZoneInfo timeZone)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddYears(years), timeZone, TimeZoneOffsetResolvers.Default);
        }

        public static DateTimeOffset AddYears(this DateTimeOffset dateTimeOffset, int years, TimeZoneInfo timeZone, TimeZoneOffsetResolver resolver)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddYears(years), timeZone, resolver);
        }

        public static DateTimeOffset AddMonths(this DateTimeOffset dateTimeOffset, int months, TimeZoneInfo timeZone)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddMonths(months), timeZone, TimeZoneOffsetResolvers.Default);
        }

        public static DateTimeOffset AddMonths(this DateTimeOffset dateTimeOffset, int months, TimeZoneInfo timeZone, TimeZoneOffsetResolver resolver)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddMonths(months), timeZone, resolver);
        }

        public static DateTimeOffset AddDays(this DateTimeOffset dateTimeOffset, int days, TimeZoneInfo timeZone)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddDays(days), timeZone, TimeZoneOffsetResolvers.Default);
        }

        public static DateTimeOffset AddDays(this DateTimeOffset dateTimeOffset, int days, TimeZoneInfo timeZone, TimeZoneOffsetResolver resolver)
        {
            return AddByDate(dateTimeOffset, dt => dt.AddDays(days), timeZone, resolver);
        }

        public static DateTimeOffset AddHours(this DateTimeOffset dateTimeOffset, double hours, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(TimeSpan.FromHours(hours), timeZone);
        }

        public static DateTimeOffset AddMinutes(this DateTimeOffset dateTimeOffset, double minutes, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(TimeSpan.FromMinutes(minutes), timeZone);
        }

        public static DateTimeOffset AddSeconds(this DateTimeOffset dateTimeOffset, double seconds, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(TimeSpan.FromSeconds(seconds), timeZone);
        }

        public static DateTimeOffset AddMilliseconds(this DateTimeOffset dateTimeOffset, double milliseconds, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(TimeSpan.FromMilliseconds(milliseconds), timeZone);
        }

        public static DateTimeOffset AddTicks(this DateTimeOffset dateTimeOffset, long ticks, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(TimeSpan.FromTicks(ticks), timeZone);
        }

        public static DateTimeOffset Subtract(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan, TimeZoneInfo timeZone)
        {
            return dateTimeOffset.Add(timeSpan.Negate(), timeZone);
        }

        public static DateTimeOffset Add(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan, TimeZoneInfo timeZone)
        {
            var t = dateTimeOffset.Add(timeSpan);
            return TimeZoneInfo.ConvertTime(t, timeZone);
        }
        
        private static DateTimeOffset AddByDate(DateTimeOffset dateTimeOffset, Func<DateTime, DateTime> operation, TimeZoneInfo timeZone, TimeZoneOffsetResolver resolver)
        {
            var dto = TimeZoneInfo.ConvertTime(dateTimeOffset, timeZone);
            var dt = operation.Invoke(dto.DateTime);
            return resolver.Invoke(dt, timeZone);
        }
    }
}
