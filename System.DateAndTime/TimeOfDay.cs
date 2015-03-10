using System.Diagnostics.Contracts;
using System.Globalization;

namespace System
{
    /// <summary>
    /// Represents a time of day, as would be read from a clock, within the range 00:00:00 to 23:59:59.9999999
    /// Has properties for working with both 12-hour and 24-hour time values.
    /// </summary>
    public struct TimeOfDay : IEquatable<TimeOfDay>
    {
        private const long MinTicks = 0L;
        private const long MaxTicks = 863999999999L;

        // Number of ticks (100ns units) since midnight
        private readonly long _ticks;

        public TimeOfDay(long ticks)
        {
            if (ticks < MinTicks || ticks > MaxTicks)
                throw new ArgumentOutOfRangeException("ticks");
            Contract.EndContractBlock();

            _ticks = ticks;
        }

        public TimeOfDay(int hours24, int minutes)
        {
            TimeSpan timeSpan = new TimeSpan(hours24, minutes, 0);
            _ticks = timeSpan.Ticks;
        }

        public TimeOfDay(int hours12, int minutes, Meridiem meridiem)
        {
            int hours24 = Hours12To24(hours12, meridiem);
            TimeSpan timeSpan = new TimeSpan(hours24, minutes, 0);
            _ticks = timeSpan.Ticks;
        }

        public TimeOfDay(int hours24, int minutes, int seconds)
        {
            TimeSpan timeSpan = new TimeSpan(hours24, minutes, seconds);
            _ticks = timeSpan.Ticks;
        }

        public TimeOfDay(int hours12, int minutes, int seconds, Meridiem meridiem)
        {
            int hours24 = Hours12To24(hours12, meridiem);
            TimeSpan timeSpan = new TimeSpan(hours24, minutes, seconds);
            _ticks = timeSpan.Ticks;
        }

        public TimeOfDay(int hours24, int minutes, int seconds, int milliseconds)
        {
            TimeSpan timeSpan = new TimeSpan(0, hours24, minutes, seconds, milliseconds);
            _ticks = timeSpan.Ticks;
        }

        public TimeOfDay(int hours12, int minutes, int seconds, int milliseconds, Meridiem meridiem)
        {
            int hours24 = Hours12To24(hours12, meridiem);
            TimeSpan timeSpan = new TimeSpan(0, hours24, minutes, seconds, milliseconds);
            _ticks = timeSpan.Ticks;
        }

        public int Hours24
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                return ts.Hours;
            }
        }

        public int Hours12
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                int hour = ts.Hours % 12;
                return hour == 0 ? 12 : hour;
            }
        }

        public Meridiem Meridiem
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                return ts.Hours < 12 ? Meridiem.AM : Meridiem.PM;
            }
        }

        public int Minutes
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                return ts.Minutes;
            }
        }

        public int Seconds
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                return ts.Seconds;
            }
        }

        public int Milliseconds
        {
            get
            {
                TimeSpan ts = new TimeSpan(_ticks);
                return ts.Milliseconds;
            }
        }

        public static TimeOfDay MinValue
        {
            get { return new TimeOfDay(MinTicks); }
        }

        public static TimeOfDay MaxValue
        {
            get { return new TimeOfDay(MaxTicks); }
        }

        public long Ticks
        {
            get { return _ticks; }
        }

        public DateTime On(Date date)
        {
            long ticks = date.DayNumber * TimeSpan.TicksPerDay + _ticks;
            return new DateTime(ticks);
        }

        public static TimeOfDay Now(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utcNow, timeZone);
            return TimeOfDayFromTimeSpan(localNow.TimeOfDay);
        }

        public static TimeOfDay NowLocal()
        {
            var localNow = DateTime.Now;
            return TimeOfDayFromTimeSpan(localNow.TimeOfDay);
        }

        public static TimeOfDay NowUtc()
        {
            var utcNow = DateTime.UtcNow;
            return TimeOfDayFromTimeSpan(utcNow.TimeOfDay);
        }

        /// <summary>
        /// Determines if a time falls within the range provided.
        /// Supports both "normal" ranges such as 10:00-12:00, and ranges that span midnight such as 23:00-01:00.
        /// </summary>
        /// <param name="startTime">The starting time of day, inclusive.</param>
        /// <param name="endTime">The ending time of day, exclusive.</param>
        /// <returns>True, if the time falls within the range, false otherwise.</returns>
        public bool IsBetween(TimeOfDay startTime, TimeOfDay endTime)
        {
            long startTicks = startTime._ticks;
            long endTicks = endTime._ticks;

            return startTicks <= endTicks
                ? (startTicks <= _ticks && endTicks > _ticks)
                : (startTicks <= _ticks || endTicks > _ticks);
        }

        /// <summary>
        /// Calculates the duration between two time values.
        /// Assumes a standard day, with no invalid or ambiguous times due to Daylight Saving Time.
        /// Supports both "normal" ranges such as 10:00-12:00, and ranges that span midnight such as 23:00-01:00.
        /// </summary>
        /// <param name="startTime">The starting time of day, inclusive.</param>
        /// <param name="endTime">The ending time of day, exclusive.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the duration between the two values.</returns>
        public static TimeSpan CalculateDuration(TimeOfDay startTime, TimeOfDay endTime)
        {
            long startTicks = startTime._ticks;
            long endTicks = endTime._ticks;

            return startTicks <= endTicks
                ? TimeSpan.FromTicks(endTicks - startTicks)
                : TimeSpan.FromTicks(endTicks - startTicks + TimeSpan.TicksPerDay);
        }

        public TimeOfDay Add(TimeSpan timeSpan)
        {
            DateTime dt = new Date(5000, 0, 0).At(this).Add(timeSpan);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public TimeOfDay AddHours(double hours)
        {
            return Add(TimeSpan.FromHours(hours));
        }

        public TimeOfDay AddMinutes(double minutes)
        {
            return Add(TimeSpan.FromMinutes(minutes));
        }

        public TimeOfDay AddSeconds(double seconds)
        {
            return Add(TimeSpan.FromSeconds(seconds));
        }

        public TimeOfDay AddMilliseconds(double milliseconds)
        {
            return Add(TimeSpan.FromMilliseconds(milliseconds));
        }

        public TimeOfDay AddTicks(long ticks)
        {
            return Add(TimeSpan.FromTicks(ticks));
        }

        public TimeOfDay Subtract(TimeSpan timeSpan)
        {
            return Add(timeSpan.Negate());
        }

        public TimeOfDay SubtractHours(double hours)
        {
            return Subtract(TimeSpan.FromHours(hours));
        }

        public TimeOfDay SubtractMinutes(double minutes)
        {
            return Subtract(TimeSpan.FromMinutes(minutes));
        }

        public TimeOfDay SubtractSeconds(double seconds)
        {
            return Subtract(TimeSpan.FromSeconds(seconds));
        }

        public TimeOfDay SubtractMilliseconds(double milliseconds)
        {
            return Subtract(TimeSpan.FromMilliseconds(milliseconds));
        }

        public TimeOfDay SubtractTicks(long ticks)
        {
            return Subtract(TimeSpan.FromTicks(ticks));
        }

        public static TimeOfDay operator +(TimeOfDay timeOfDay, TimeSpan timeSpan)
        {
            return timeOfDay.Add(timeSpan);
        }

        public static TimeOfDay operator -(TimeOfDay timeOfDay, TimeSpan timeSpan)
        {
            return timeOfDay.Subtract(timeSpan);
        }

        public bool Equals(TimeOfDay other)
        {
            return _ticks == other._ticks;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TimeOfDay && Equals((TimeOfDay) obj);
        }

        public override int GetHashCode()
        {
            return _ticks.GetHashCode();
        }

        public override string ToString()
        {
            return DateTime.MinValue.AddTicks(_ticks).ToString("T");
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return DateTime.MinValue.AddTicks(_ticks).ToString("T", formatProvider);
        }

        public string ToString(string format)
        {
            return DateTime.MinValue.AddTicks(_ticks).ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return DateTime.MinValue.AddTicks(_ticks).ToString(format, formatProvider);
        }

        public static TimeOfDay Parse(string s)
        {
            DateTime dt = DateTime.Parse(s, null, DateTimeStyles.NoCurrentDateDefault);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay Parse(string s, IFormatProvider provider)
        {
            DateTime dt = DateTime.Parse(s, provider, DateTimeStyles.NoCurrentDateDefault);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay ParseExact(string s, string format, IFormatProvider provider)
        {
            DateTime dt = DateTime.ParseExact(s, format, provider, DateTimeStyles.NoCurrentDateDefault);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay ParseExact(string s, string[] formats, IFormatProvider provider)
        {
            DateTime dt = DateTime.ParseExact(s, formats, provider, DateTimeStyles.NoCurrentDateDefault);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static bool TryParse(string s, out TimeOfDay time)
        {
            DateTime dt;
            if (!DateTime.TryParse(s, null, DateTimeStyles.NoCurrentDateDefault, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool TryParse(string s, IFormatProvider provider, out TimeOfDay time)
        {
            DateTime dt;
            if (!DateTime.TryParse(s, provider, DateTimeStyles.NoCurrentDateDefault, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool TryParseExact(string s, string format, IFormatProvider provider, out TimeOfDay time)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(s, format, provider, DateTimeStyles.NoCurrentDateDefault, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, out TimeOfDay time)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(s, formats, provider, DateTimeStyles.NoCurrentDateDefault, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool operator ==(TimeOfDay left, TimeOfDay right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimeOfDay left, TimeOfDay right)
        {
            return !left.Equals(right);
        }

        public static implicit operator TimeOfDay(TimeSpan timeSpan)
        {
            // This is useful such that existing items like DateTime.TimeOfDay can be assigned to a TimeOfDay type.

            long ticks = timeSpan.Ticks;
            if (ticks < 0 || ticks >= TimeSpan.TicksPerDay)
                throw new ArgumentOutOfRangeException("timeSpan");
            Contract.EndContractBlock();

            return new TimeOfDay(ticks);
        }

        private static int Hours12To24(int hours12, Meridiem meridiem)
        {
            if (hours12 < 1 || hours12 > 12)
                throw new ArgumentOutOfRangeException("hours12");

            if (!Enum.IsDefined(typeof(Meridiem), meridiem))
                throw new ArgumentOutOfRangeException("meridiem");
            
            Contract.EndContractBlock();

            return meridiem == Meridiem.AM
                ? (hours12 == 12 ? 0 : hours12)
                : (hours12 == 12 ? 12 : hours12 + 12);
        }

        private static TimeOfDay TimeOfDayFromTimeSpan(TimeSpan timeSpan)
        {
            return new TimeOfDay(timeSpan.Ticks);
        }
    }
}
