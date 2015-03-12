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
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;   // 10,000,000
        private const long TicksPerMinute = TicksPerSecond * 60;         // 600,000,000
        private const long TicksPerHour = TicksPerMinute * 60;        // 36,000,000,000
        private const long TicksPerDay = TicksPerHour * 24;          // 864,000,000,000

        private const long MinTicks = 0L;
        private const long MaxTicks = 863999999999L;

        public static readonly TimeOfDay MinValue = new TimeOfDay(MinTicks);
        public static readonly TimeOfDay MaxValue = new TimeOfDay(MaxTicks);

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
            if (hours24 < 0 || hours24 > 23) throw new ArgumentOutOfRangeException("hours24");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            Contract.EndContractBlock();

            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute;
        }

        public TimeOfDay(int hours12, int minutes, Meridiem meridiem)
        {
            if (hours12 < 1 || hours12 > 12) throw new ArgumentOutOfRangeException("hours12");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hours12, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute;
        }

        public TimeOfDay(int hours24, int minutes, int seconds)
        {
            if (hours24 < 0 || hours24 > 23) throw new ArgumentOutOfRangeException("hours24");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            if (seconds < 0 || seconds > 59) throw new ArgumentOutOfRangeException("seconds");
            Contract.EndContractBlock();

            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute +
                     seconds * TicksPerSecond;
        }

        public TimeOfDay(int hours12, int minutes, int seconds, Meridiem meridiem)
        {
            if (hours12 < 1 || hours12 > 12) throw new ArgumentOutOfRangeException("hours12");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            if (seconds < 0 || seconds > 59) throw new ArgumentOutOfRangeException("seconds");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hours12, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute +
                     seconds * TicksPerSecond;
        }

        public TimeOfDay(int hours24, int minutes, int seconds, int milliseconds)
        {
            if (hours24 < 0 || hours24 > 23) throw new ArgumentOutOfRangeException("hours24");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            if (seconds < 0 || seconds > 59) throw new ArgumentOutOfRangeException("seconds");
            if (milliseconds < 0 || milliseconds > 999) throw new ArgumentOutOfRangeException("milliseconds");
            Contract.EndContractBlock();

            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute +
                     seconds * TicksPerSecond +
                     milliseconds * TicksPerMillisecond;
        }

        public TimeOfDay(int hours12, int minutes, int seconds, int milliseconds, Meridiem meridiem)
        {
            if (hours12 < 1 || hours12 > 12) throw new ArgumentOutOfRangeException("hours12");
            if (minutes < 0 || minutes > 59) throw new ArgumentOutOfRangeException("minutes");
            if (seconds < 0 || seconds > 59) throw new ArgumentOutOfRangeException("seconds");
            if (milliseconds < 0 || milliseconds > 999) throw new ArgumentOutOfRangeException("milliseconds");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hours12, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minutes * TicksPerMinute +
                     seconds * TicksPerSecond +
                     milliseconds * TicksPerMillisecond;
        }

        public int Hours24
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 23);

                return (int)((_ticks / TicksPerHour) % 24);
            }
        }

        public int Hours12
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 12);

                int hour = Hours24 % 12;
                return hour == 0 ? 12 : hour;
            }
        }

        public Meridiem Meridiem
        {
            get
            {
                return Hours24 < 12 ? Meridiem.AM : Meridiem.PM;
            }
        }

        public int Minutes
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 59);

                return (int)((_ticks / TicksPerMinute) % 60);
            }
        }

        public int Seconds
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 59);

                return (int)((_ticks / TicksPerSecond) % 60);
            }
        }

        public int Milliseconds
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 999);

                return (int)((_ticks / TicksPerMillisecond) % 1000);
            }
        }

        public long Ticks
        {
            get
            {
                Contract.Ensures(Contract.Result<long>() >= MinTicks);
                Contract.Ensures(Contract.Result<long>() <= MaxTicks);

                return _ticks;
            }
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
            return AddTicks(timeSpan.Ticks);
        }

        public TimeOfDay AddHours(double hours)
        {
            return AddTicks((long)(hours * TicksPerHour));
        }

        public TimeOfDay AddMinutes(double minutes)
        {
            return AddTicks((long)(minutes * TicksPerMinute));
        }

        public TimeOfDay AddSeconds(double seconds)
        {
            return AddTicks((long)(seconds * TicksPerSecond));
        }

        public TimeOfDay AddMilliseconds(double milliseconds)
        {
            return AddTicks((long)(milliseconds * TicksPerMillisecond));
        }

        public TimeOfDay AddTicks(long ticks)
        {
            long t = (_ticks + TicksPerDay + (ticks % TicksPerDay)) % TicksPerDay;
            return new TimeOfDay(t);
        }

        public TimeOfDay Subtract(TimeSpan timeSpan)
        {
            return AddTicks(-timeSpan.Ticks);
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
            return obj is TimeOfDay && Equals((TimeOfDay)obj);
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
