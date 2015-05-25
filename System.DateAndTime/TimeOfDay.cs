using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System
{
    /// <summary>
    /// Represents a time of day, as would be read from a clock, within the range 00:00:00 to 23:59:59.9999999
    /// Has properties for working with both 12-hour and 24-hour time values.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    [XmlSchemaProvider("GetSchema")]
    public struct TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>, IComparable, IFormattable, IXmlSerializable
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;   // 10,000,000
        private const long TicksPerMinute = TicksPerSecond * 60;         // 600,000,000
        private const long TicksPerHour = TicksPerMinute * 60;        // 36,000,000,000
        private const long TicksPerDay = TicksPerHour * 24;          // 864,000,000,000

        private const long MinTicks = 0L;
        private const long MaxTicks = 863999999999L;

        /// <summary>
        /// Represents the smallest possible value of <see cref="TimeOfDay"/>. This field is read-only.
        /// </summary>
        public static readonly TimeOfDay MinValue = new TimeOfDay(MinTicks);
        
        /// <summary>
        /// Represents the largest possible value of <see cref="TimeOfDay"/>. This field is read-only.
        /// </summary>
        public static readonly TimeOfDay MaxValue = new TimeOfDay(MaxTicks);

        // Number of ticks (100ns units) since midnight at the beginning of a standard 24-hour day.
        // NOTE: This is the only field in this structure.
        private readonly long _ticks;

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to a specified number of ticks.
        /// </summary>
        /// <param name="ticks">
        /// A time expressed in the number of 100-nanosecond intervals that have elapsed since midnight (00:00:00),
        /// without regard to daylight saving time transitions.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ticks"/> is out of the range supported by the <see cref="TimeOfDay"/> object.
        /// </exception>
        public TimeOfDay(long ticks)
        {
            if (ticks < MinTicks || ticks > MaxTicks)
                throw new ArgumentOutOfRangeException("ticks");
            Contract.EndContractBlock();

            _ticks = ticks;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour and minute.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 0 or greater than 23.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// </exception>
        public TimeOfDay(int hour, int minute)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            Contract.EndContractBlock();

            _ticks = hour * TicksPerHour +
                     minute * TicksPerMinute;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour, minute, and meridiem, using the hours of a 12-hour clock.
        /// </summary>
        /// <param name="hour">The hours (1 through 12).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="meridiem">The meridiem, either <see cref="System.Meridiem.AM"/>,
        /// or <see cref="System.Meridiem.PM"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 1 or greater than 12.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// </exception>
        public TimeOfDay(int hour, int minute, Meridiem meridiem)
        {
            if (hour < 1 || hour > 12) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hour, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minute * TicksPerMinute;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour, minute, and second, using the hours of a 24-hour clock.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 0 or greater than 23.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="second"/> is less than 0 or greater than 59.
        /// </exception>
        public TimeOfDay(int hour, int minute, int second)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            Contract.EndContractBlock();

            _ticks = hour * TicksPerHour +
                     minute * TicksPerMinute +
                     second * TicksPerSecond;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour, minute, second, and meridiem, using the hours of a 12-hour clock.
        /// </summary>
        /// <param name="hour">The hours (1 through 12).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <param name="meridiem">The meridiem, either <see cref="System.Meridiem.AM"/>,
        /// or <see cref="System.Meridiem.PM"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 1 or greater than 12.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="second"/> is less than 0 or greater than 59.
        /// </exception>
        public TimeOfDay(int hour, int minute, int second, Meridiem meridiem)
        {
            if (hour < 1 || hour > 12) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hour, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minute * TicksPerMinute +
                     second * TicksPerSecond;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour, minute, second, and millisecond, using the hours of a 24-hour clock.
        /// </summary>
        /// <param name="hour">The hours (0 through 23).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <param name="millisecond">The milliseconds (0 through 999).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 0 or greater than 23.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="second"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="millisecond"/> is less than 0 or greater than 999.
        /// </exception>
        public TimeOfDay(int hour, int minute, int second, int millisecond)
        {
            if (hour < 0 || hour > 23) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            if (millisecond < 0 || millisecond > 999) throw new ArgumentOutOfRangeException("millisecond");
            Contract.EndContractBlock();

            _ticks = hour * TicksPerHour +
                     minute * TicksPerMinute +
                     second * TicksPerSecond +
                     millisecond * TicksPerMillisecond;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeOfDay"/> structure to the specified
        /// hour, minute, second, millisecond, and meridiem, using the hours of a 12-hour clock.
        /// </summary>
        /// <param name="hour">The hours (1 through 12).</param>
        /// <param name="minute">The minutes (0 through 59).</param>
        /// <param name="second">The seconds (0 through 59).</param>
        /// <param name="millisecond">The milliseconds (0 through 999).</param>
        /// <param name="meridiem">The meridiem, either <see cref="System.Meridiem.AM"/>,
        /// or <see cref="System.Meridiem.PM"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hour"/> is less than 1 or greater than 12.
        /// <para>-or-</para>
        /// <paramref name="minute"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="second"/> is less than 0 or greater than 59.
        /// <para>-or-</para>
        /// <paramref name="millisecond"/> is less than 0 or greater than 999.
        /// </exception>
        public TimeOfDay(int hour, int minute, int second, int millisecond, Meridiem meridiem)
        {
            if (hour < 1 || hour > 12) throw new ArgumentOutOfRangeException("hour");
            if (minute < 0 || minute > 59) throw new ArgumentOutOfRangeException("minute");
            if (second < 0 || second > 59) throw new ArgumentOutOfRangeException("second");
            if (millisecond < 0 || millisecond > 999) throw new ArgumentOutOfRangeException("millisecond");
            if (meridiem < Meridiem.AM || meridiem > Meridiem.PM) throw new ArgumentOutOfRangeException("meridiem");
            Contract.EndContractBlock();

            int hours24 = Hours12To24(hour, meridiem);
            _ticks = hours24 * TicksPerHour +
                     minute * TicksPerMinute +
                     second * TicksPerSecond +
                     millisecond * TicksPerMillisecond;
        }

        /// <summary>
        /// Gets the hour component of the time represented by this instance, using the hours of a 24-hour clock.
        /// </summary>
        /// <value>The hour component, expressed as a value between 0 and 23.</value>
        public int Hour
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 23);

                return (int)((_ticks / TicksPerHour) % 24);
            }
        }

        /// <summary>
        /// Gets the hour component of the time represented by this instance, using the hours of a 12-hour clock.
        /// </summary>
        /// <value>The hour component, expressed as a value between 1 and 12.</value>
        public int HourOf12HourClock
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 12);

                int hour = Hour % 12;
                return hour == 0 ? 12 : hour;
            }
        }

        /// <summary>
        /// Gets the meridiem (AM or PM) of the time represented by this instance.
        /// The meridiem can be used inconjunction with the <see cref="HourOf12HourClock"/> property
        /// to represent this instance's time on a 12-hour clock.
        /// </summary>
        /// <value>An enumerated constant that indicates the meridiem of this <see cref="TimeOfDay"/> value.</value>
        /// <remarks>
        /// Though commonly used in English, these abbreviations derive from Latin.
        /// AM is an abbreviation for "Ante Meridiem", meaning "before mid-day".
        /// PM is an abbreviation for "Post Meridiem", meaning "after mid-day".
        /// </remarks>
        public Meridiem Meridiem
        {
            get
            {
                return Hour < 12 ? Meridiem.AM : Meridiem.PM;
            }
        }

        /// <summary>
        /// Gets the minute component of the time represented by this instance.
        /// </summary>
        /// <value>The minute component, expressed as a value between 0 and 59.</value>
        public int Minute
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 59);

                return (int)((_ticks / TicksPerMinute) % 60);
            }
        }

        /// <summary>
        /// Gets the second component of the time represented by this instance.
        /// </summary>
        /// <value>The second component, expressed as a value between 0 and 59.</value>
        public int Second
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 59);

                return (int)((_ticks / TicksPerSecond) % 60);
            }
        }

        /// <summary>
        /// Gets the millisecond component of the time represented by this instance.
        /// </summary>
        /// <value>The millisecond component, expressed as a value between 0 and 999.</value>
        public int Millisecond
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() <= 999);
                
                return (int)((_ticks / TicksPerMillisecond) % 1000);
            }
        }

        /// <summary>
        /// Gets the number of ticks that represent the time of this instance.
        /// </summary>
        /// <value>
        /// The number of ticks that represent the time of this instance.
        /// The value is between <c>TimeOfDay.MinValue.Ticks</c> and <c>TimeOfDay.MaxValue.Ticks</c>.
        /// </value>
        /// <remarks>
        /// Each tick is a 100-nanosecond interval.  Collectively, they represent the time that has
        /// elapsed since midnight (00:00:00), without regard to daylight saving time transitions.
        /// </remarks>
        public long Ticks
        {
            get
            {
                Contract.Ensures(Contract.Result<long>() >= MinTicks);
                Contract.Ensures(Contract.Result<long>() <= MaxTicks);

                return _ticks;
            }
        }

        /// <summary>
        /// Creates a <see cref="DateTime"/> object from the current <see cref="TimeOfDay"/> and the specified <see cref="Date"/>.
        /// The resulting value has a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <remarks>
        /// Since neither <see cref="Date"/> or <see cref="TimeOfDay"/> keep track of <see cref="DateTimeKind"/>,
        /// recognize that the <see cref="DateTime"/> produced by <c>TimeOfDay.Now.On(Date.Today)</c> will have
        /// <see cref="DateTimeKind.Unspecified"/>, rather than then <see cref="DateTimeKind.Local"/> that would be
        /// given by <c>DateTime.Now</c>.
        /// <para>The same applies for <see cref="DateTimeKind.Utc"/>.</para>
        /// </remarks>
        public DateTime On(Date date)
        {
            long ticks = date.DayNumber * TicksPerDay + _ticks;
            return new DateTime(ticks);
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object that is set to the current time in the specified time zone.
        /// </summary>
        /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/> instance.</param>
        /// <returns>The current <see cref="TimeOfDay"/> for the specified time zone.</returns>
        public static TimeOfDay NowInTimeZone(TimeZoneInfo timeZoneInfo)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo);
            return TimeOfDayFromTimeSpan(localNow.TimeOfDay);
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object that is set to the current time,
        /// expressed in this computer's local time zone.
        /// </summary>
        /// <value>An object whose value is the current local time.</value>
        public static TimeOfDay Now
        {
            get
            {
                DateTime localNow = DateTime.Now;
                return TimeOfDayFromTimeSpan(localNow.TimeOfDay);
            }
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object that is set to the current time,
        /// expressed as Coordinated Universal Time (UTC).
        /// </summary>
        /// <value>An object whose value is the current UTC time.</value>
        public static TimeOfDay UtcNow
        {
            get
            {
                DateTime utcNow = DateTime.UtcNow;
                return TimeOfDayFromTimeSpan(utcNow.TimeOfDay);
            }
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
            return TimeSpan.FromTicks((endTime._ticks - startTime._ticks + TicksPerDay) % TicksPerDay);
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified amount of time. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="timeSpan">The amount of time to adjust by. The value can be negative or positive.</param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="timeSpan"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:00 plus two hours is 01:00.
        /// </remarks>
        public TimeOfDay Add(TimeSpan timeSpan)
        {
            return AddTicks(timeSpan.Ticks);
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified number of hours. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="hours">The number of hours to adjust by. The value can be negative or positive.</param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="hours"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:00 plus one hour is 00:00.
        /// </remarks>
        public TimeOfDay AddHours(double hours)
        {
            return AddTicks((long)(hours * TicksPerHour));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified number of minutes. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="minutes">The number of minutes to adjust by. The value can be negative or positive.</param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="minutes"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:59 plus one minute is 00:00.
        /// </remarks>
        public TimeOfDay AddMinutes(double minutes)
        {
            return AddTicks((long)(minutes * TicksPerMinute));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified number of seconds. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="seconds">The number of seconds to adjust by. The value can be negative or positive.</param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="seconds"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:59:59 plus one second is 00:00:00.
        /// </remarks>
        public TimeOfDay AddSeconds(double seconds)
        {
            return AddTicks((long)(seconds * TicksPerSecond));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified number of milliseconds. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="milliseconds">
        /// The number of milliseconds to adjust by. The value can be negative or positive.
        /// </param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="milliseconds"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:59:59.9990000 plus one millisecond is 00:00:00.0000000.
        /// </remarks>
        public TimeOfDay AddMilliseconds(double milliseconds)
        {
            return AddTicks((long)(milliseconds * TicksPerMillisecond));
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified number of ticks. Positive values will move the time forward; negative values will move the
        /// time backwards.
        /// </summary>
        /// <param name="ticks">
        /// The number of ticks to adjust by. The value can be negative or positive.
        /// A tick is a unit of time equal to 100 nanoseconds.
        /// </param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="ticks"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:59:59.9999999 plus one tick is 00:00:00.0000000.
        /// </remarks>
        public TimeOfDay AddTicks(long ticks)
        {
            long t = (_ticks + TicksPerDay + (ticks % TicksPerDay)) % TicksPerDay;
            return new TimeOfDay(t);
        }

        /// <summary>
        /// Gets a <see cref="TimeOfDay"/> object whose value is ahead or behind the value of this instance by the
        /// specified amount of time. Positive values will move the time backwards; negative values will move the
        /// time forward.  This is equivalent to calling <c>Add(timeSpan.Negate())</c>.
        /// </summary>
        /// <param name="timeSpan">The amount of time to adjust by. The value can be negative or positive.</param>
        /// <returns>
        /// A new <see cref="TimeOfDay"/> object which is the result of adjusting this instance by the
        /// <paramref name="timeSpan"/> specified.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries backwards
        /// into the previous day.  For example, 01:00 minus two hours is 23:00.
        /// </remarks>
        public TimeOfDay Subtract(TimeSpan timeSpan)
        {
            return AddTicks(-timeSpan.Ticks);
        }


        /// <summary>
        /// Adds a specified time interval to a specified time, yielding a new time.
        /// </summary>
        /// <param name="timeOfDay">The time of day value to add to.</param>
        /// <param name="timeSpan">The time interval to add.</param>
        /// <returns>
        /// A <see cref="TimeOfDay"/> object which is the result of adding the <paramref name="timeSpan"/>
        /// specified to the <paramref name="timeOfDay"/> provided.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries forward
        /// into the next day.  For example, 23:00 plus two hours is 01:00.
        /// </remarks>
        public static TimeOfDay operator +(TimeOfDay timeOfDay, TimeSpan timeSpan)
        {
            return timeOfDay.Add(timeSpan);
        }

        /// <summary>
        /// Subtracts a specified time interval to a specified time, yielding a new time.
        /// </summary>
        /// <param name="timeOfDay">The time of day value to subtract from.</param>
        /// <param name="timeSpan">The time interval to subtract.</param>
        /// <returns>
        /// A <see cref="TimeOfDay"/> object which is the result of subtracting the <paramref name="timeSpan"/>
        /// specified from the <paramref name="timeOfDay"/> provided.
        /// </returns>
        /// <remarks>
        /// The time is modeled on a circular 24-hour clock.  When a value crosses midnight, it carries backwards
        /// into the previous day.  For example, 01:00 minus two hours is 23:00.
        /// </remarks>
        public static TimeOfDay operator -(TimeOfDay timeOfDay, TimeSpan timeSpan)
        {
            return timeOfDay.Subtract(timeSpan);
        }

        /// <summary>
        /// Compares two instances of <see cref="TimeOfDay"/> and returns an integer that indicates whether the first
        /// instance is earlier than, the same as, or later than the second instance, within the same day.
        /// </summary>
        /// <param name="t1">The first object to compare.</param>
        /// <param name="t2">The second object to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of <paramref name="t1"/> and <paramref name="t2"/>.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item>
        ///   <term>Less than zero</term>
        ///   <term><paramref name="t1"/> is earlier than <paramref name="t2"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Zero</term>
        ///   <term><paramref name="t1"/> is the same as <paramref name="t2"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Greater than zero</term>
        ///   <term><paramref name="t1"/> is later than <paramref name="t2"/>.</term>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This operation considers all time values to be contained within the same day.  If you need to compare
        /// time values that cross midnight into a different day, use the <see cref="IsBetween"/> method instead.
        /// </remarks>
        public static int Compare(TimeOfDay t1, TimeOfDay t2)
        {
            if (t1._ticks > t2._ticks) return 1;
            if (t1._ticks < t2._ticks) return -1;
            return 0;
        }

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="TimeOfDay"/> value and returns an integer
        /// that indicates whether this instance is earlier than, the same as, or later than the specified
        /// <see cref="TimeOfDay"/> value, within the same day.
        /// </summary>
        /// <param name="value">The object to compare to the current instance.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and the <paramref name="value"/> parameter.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item>
        ///   <term>Less than zero</term>
        ///   <term>This instance is earlier than <paramref name="value"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Zero</term>
        ///   <term>This instance is the same as <paramref name="value"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Greater than zero</term>
        ///   <term>This instance is later than <paramref name="value"/>.</term>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This operation considers all time values to be contained within the same day.  If you need to compare
        /// time values that cross midnight into a different day, use the <see cref="IsBetween"/> method instead.
        /// </remarks>
        public int CompareTo(TimeOfDay value)
        {
            return Compare(this, value);
        }

        /// <summary>
        /// Compares the value of this instance to a specified object that contains a <see cref="TimeOfDay"/> value and
        /// returns an integer that indicates whether this instance is earlier than, the same as, or later than the
        /// specified <see cref="TimeOfDay"/> value, within the same day.
        /// </summary>
        /// <param name="value">The object to compare to the current instance.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and the <paramref name="value"/> parameter.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item>
        ///   <term>Less than zero</term>
        ///   <term>This instance is earlier than <paramref name="value"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Zero</term>
        ///   <term>This instance is earlier than <paramref name="value"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Greater than zero</term>
        ///   <term>
        ///     This instance is later than <paramref name="value"/>,
        ///     or <paramref name="value"/> is <c>null</c>.
        ///   </term>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This operation considers all time values to be contained within the same day.  If you need to compare
        /// time values that cross midnight into a different day, use the <see cref="IsBetween"/> method instead.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not a <see cref="TimeOfDay"/>.
        /// </exception>
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (!(value is TimeOfDay))
                throw new ArgumentException();

            return Compare(this, (TimeOfDay)value);
        }

        /// <summary>
        /// Returns a value indicating whether two <see cref="TimeOfDay"/> instances have the same time value.
        /// </summary>
        /// <param name="t1">The first object to compare.</param>
        /// <param name="t2">The second object to compare.</param>
        /// <returns><c>true</c> if the two values are equal; otherwise, <c>false</c>.</returns>
        public static bool Equals(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.Equals(t2);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified
        /// <see cref="TimeOfDay"/> instance.
        /// </summary>
        /// <param name="value">The other <see cref="TimeOfDay"/> object to compare against this instance.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> parameter equals the value of this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TimeOfDay value)
        {
            return _ticks == value._ticks;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is an instance of <see cref="TimeOfDay"/>
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (ReferenceEquals(null, value)) return false;
            return value is TimeOfDay && Equals((TimeOfDay)value);
        }

        /// <summary>
        /// Returns the hash code of this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <remarks>
        /// The hash code of a <see cref="TimeOfDay"/> object is the same as the hash code of
        /// its <see cref="Ticks"/> value.
        /// </remarks>
        public override int GetHashCode()
        {
            return _ticks.GetHashCode();
        }

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return DateTime.MinValue.AddTicks(_ticks).ToString("T");
        }

        public string ToString(IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return DateTime.MinValue.AddTicks(_ticks).ToString("T", provider);
        }

        public string ToString(string format)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            format = NormalizeTimeFormat(format);
            return DateTime.MinValue.AddTicks(_ticks).ToString(format);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            format = NormalizeTimeFormat(format);
            return DateTime.MinValue.AddTicks(_ticks).ToString(format, provider);
        }

        /// <summary>
        /// Converts the value of the current <see cref="TimeOfDay"/> object to its equivalent
        /// long time string representation.
        /// </summary>
        /// <returns>A string that contains the long time string representation of the
        /// current <see cref="TimeOfDay"/> object.</returns>
        /// <remarks>The value of the current <see cref="TimeOfDay"/> object is formatted
        /// using the pattern defined by the <see cref="DateTimeFormatInfo.LongTimePattern" />
        /// property associated with the current thread culture.</remarks>
        public string ToLongTimeString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
        }

        /// <summary>
        /// Converts the value of the current <see cref="TimeOfDay"/> object to its equivalent
        /// short time string representation.
        /// </summary>
        /// <returns>A string that contains the short time string representation of the
        /// current <see cref="TimeOfDay"/> object.</returns>
        /// <remarks>The value of the current <see cref="TimeOfDay"/> object is formatted
        /// using the pattern defined by the <see cref="DateTimeFormatInfo.ShortTimePattern" />
        /// property associated with the current thread culture.</remarks>
        public string ToShortTimeString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
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

        public static TimeOfDay Parse(string s, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            DateTime dt = DateTime.Parse(s, provider, DateTimeStyles.NoCurrentDateDefault | styles);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay ParseExact(string s, string format, IFormatProvider provider)
        {
            format = NormalizeTimeFormat(format);
            DateTime dt = DateTime.ParseExact(s, format, provider, DateTimeStyles.NoCurrentDateDefault);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            format = NormalizeTimeFormat(format);
            DateTime dt = DateTime.ParseExact(s, format, provider, DateTimeStyles.NoCurrentDateDefault | styles);
            return TimeOfDayFromTimeSpan(dt.TimeOfDay);
        }

        public static TimeOfDay ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            for (int i = 0; i < formats.Length; i++)
            {
                formats[i] = NormalizeTimeFormat(formats[i]);
            }

            DateTime dt = DateTime.ParseExact(s, formats, provider, DateTimeStyles.NoCurrentDateDefault | styles);
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

        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out TimeOfDay time)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            DateTime dt;
            if (!DateTime.TryParse(s, provider, DateTimeStyles.NoCurrentDateDefault | styles, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles styles, out TimeOfDay time)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            format = NormalizeTimeFormat(format);

            DateTime dt;
            if (!DateTime.TryParseExact(s, format, provider, DateTimeStyles.NoCurrentDateDefault | styles, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles styles, out TimeOfDay time)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            for (int i = 0; i < formats.Length; i++)
            {
                formats[i] = NormalizeTimeFormat(formats[i]);
            }

            DateTime dt;
            if (!DateTime.TryParseExact(s, formats, provider, DateTimeStyles.NoCurrentDateDefault | styles, out dt))
            {
                time = default(TimeOfDay);
                return false;
            }

            time = TimeOfDayFromTimeSpan(dt.TimeOfDay);
            return true;
        }

        public static bool operator ==(TimeOfDay t1, TimeOfDay t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(TimeOfDay t1, TimeOfDay t2)
        {
            return !t1.Equals(t2);
        }

        public static bool operator >(TimeOfDay t1, TimeOfDay t2)
        {
            return t1._ticks > t2._ticks;
        }

        public static bool operator >=(TimeOfDay t1, TimeOfDay t2)
        {
            return t1._ticks >= t2._ticks;
        }

        public static bool operator <(TimeOfDay t1, TimeOfDay t2)
        {
            return t1._ticks < t2._ticks;
        }

        public static bool operator <=(TimeOfDay t1, TimeOfDay t2)
        {
            return t1._ticks <= t2._ticks;
        }

        /// <summary>
        /// Implicitly casts a <see cref="TimeSpan"/> object to a <see cref="TimeOfDay"/> by returning a new
        /// <see cref="TimeOfDay"/> object that has the equivalent hours, minutes, seconds, and fractional seconds
        /// components.  This is useful when using APIs that express a time-of-day as the elapsed time since
        /// midnight, such that their values can be assigned to a variable having a <see cref="TimeOfDay"/> type.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/> value representing the time elapsed since midnight,
        /// without regard to daylight saving time transitions.</param>
        /// <returns>A newly constructed <see cref="TimeOfDay"/> object with an equivalent value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeSpan"/> is either negative, or greater than <c>23:59:59.9999999</c>, and thus cannot be
        /// mapped to a <see cref="TimeOfDay"/>.
        /// </exception>
        /// <remarks>
        /// Fundamentally, a time-of-day and an elapsed-time are two different concepts.  In previous versions
        /// of the .NET framework, the <see cref="TimeOfDay"/> type did not exist, and thus several time-of-day
        /// values were represented by <see cref="TimeSpan"/> values erroneously.  For example, the
        /// <see cref="DateTime.TimeOfDay"/> property returns a value having a <see cref="TimeSpan"/> type.
        /// This implicit cast operator allows those APIs to be naturally used with <see cref="TimeOfDay"/>.
        /// <para>
        /// Also note that the input <paramref name="timeSpan"/> might actually *not* accurately represent the
        /// "time elapsed since midnight" on days containing a daylight saving time transition.
        /// </para>
        /// </remarks>
        public static implicit operator TimeOfDay(TimeSpan timeSpan)
        {
            long ticks = timeSpan.Ticks;
            if (ticks < 0 || ticks >= TicksPerDay)
                throw new ArgumentOutOfRangeException("timeSpan");
            Contract.EndContractBlock();

            return new TimeOfDay(ticks);
        }

        /// <summary>
        /// Enables explicit casting of a <see cref="TimeOfDay"/> object to a <see cref="TimeSpan"/> by returning a new
        /// <see cref="TimeSpan"/> object that has the equivalent hours, minutes, seconds, and fractional seconds
        /// components.  This is useful when using APIs that express a time-of-day as the elapsed time since
        /// midnight, such that a <see cref="TimeOfDay"/> type can be passed to a method expecting a
        /// <see cref="TimeSpan"/> parameter as a time-of-day.
        /// </summary>
        /// <param name="timeOfDay">A <see cref="TimeOfDay"/> value.</param>
        /// <returns>
        /// A newly constructed <see cref="TimeSpan"/> object representing the time elapsed since midnight, without
        /// regard to daylight saving time transitions.
        /// </returns>
        public static explicit operator TimeSpan(TimeOfDay timeOfDay)
        {
            return new TimeSpan(timeOfDay.Ticks);
        }

        /// <summary>
        /// Converts the time from a 12-hour-clock representation to a 24-hour-clock representation.
        /// </summary>
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

        /// <summary>
        /// Constructs a <see cref="TimeOfDay"/> from a <see cref="TimeSpan"/> representing the time elapsed since
        /// midnight, without regard to daylight saving time transitions.
        /// </summary>
        private static TimeOfDay TimeOfDayFromTimeSpan(TimeSpan timeSpan)
        {
            return new TimeOfDay(timeSpan.Ticks);
        }

        /// <summary>
        /// Normalizes a format string that has standard or custom date/time formats,
        /// such that the formatted output can only contain a time-of-day when applied.
        /// </summary>
        /// <exception cref="FormatException">
        /// The format string contained a format specifier that is only applicable
        /// when a date would be part of the formatted output.
        /// </exception>
        private static string NormalizeTimeFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return "T";
            }

            // standard formats
            if (format.Length == 1)
            {
                // pass-through formats
                if ("Tt".Contains(format))
                    return format;

                // ISO formats
                if (format == "s")
                    return "HH:mm:ss";

                if ("Oo".Contains(format))
                    return "HH:mm:ss.fffffff";


                // All other standard DateTime formats are invalid for TimeOfDay
                throw new FormatException();
            }

            // custom format - test for date components or embedded standard date formats
            // except when escaped by preceding \ or enclosed in "" or '' quotes

            var filtered = Regex.Replace(format, @"(\\.)|("".*"")|('.*')", String.Empty);
            if (Regex.IsMatch(filtered, "([dKMyz/]+)|(%[dDfFgGmMrRuUyY]+)"))
                throw new FormatException();

            // custom format with embedded standard format(s) - ISO replacement
            format = format.Replace("%s", "HH:mm:ss");
            format = Regex.Replace(format, @"(%[Oo])", "HH:mm:ss.fffffff");

            // pass through
            return format;
        }

        /// <summary>
        /// Gets a <see cref="XmlQualifiedName"/> that represents the <c>xs:time</c> type of the
        /// W3C XML Schema Definition (XSD) specification.
        /// </summary>
        /// <remarks>
        /// This is required to support the <see cref="XmlSchemaProviderAttribute"/> applied to this structure.
        /// </remarks>
        public static XmlQualifiedName GetSchema(object xs)
        {
            return new XmlQualifiedName("time", "http://www.w3.org/2001/XMLSchema");
        }

        /// <summary>
        /// Required by the <see cref="IXmlSerializable"/> interface.
        /// </summary>
        /// <returns><c>null</c></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates a <see cref="TimeOfDay"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
        /// <remarks>
        /// An <c>xs:time</c> uses the ISO-8601 extended time format, with up to seven decimal places of fractional
        /// seconds.  The equivalent .NET Framework format string is <c>HH:mm:ss.FFFFFFF</c>.
        /// </remarks>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var s = reader.NodeType == XmlNodeType.Element
                ? reader.ReadElementContentAsString()
                : reader.ReadContentAsString();

            TimeOfDay t;
            if (!TryParseExact(s, "HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out t))
            {
                throw new FormatException();
            }

            this = t;
        }

        /// <summary>
        /// Converts a <see cref="TimeOfDay"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
        /// <remarks>
        /// An <c>xs:time</c> uses the ISO-8601 extended time format, with up to seven decimal places of fractional
        /// seconds.  The equivalent .NET Framework format string is <c>HH:mm:ss.FFFFFFF</c>.
        /// </remarks>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
        }
    }
}
