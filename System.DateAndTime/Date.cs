using System.Diagnostics.Contracts;
using System.Globalization;

namespace System
{
    /// <summary>
    /// Represents a whole date, having a year, month and day component.
    /// All values are in the proleptic Gregorian (ISO8601) calendar system.
    /// </summary>
    public struct Date : IEquatable<Date>, IComparable<Date>, IComparable, IFormattable
    {
        private const int MinDayNumber = 0;
        private const int MaxDayNumber = 3652058;

        // Number of whole days since 0001-01-01 (which is day 0)
        private readonly int _dayNumber;

        public Date(int dayNumber)
        {
            if (dayNumber < MinDayNumber || dayNumber > MaxDayNumber)
                throw new ArgumentOutOfRangeException("dayNumber");
            Contract.EndContractBlock();

            _dayNumber = dayNumber;
        }

        public Date(int year, int month, int day)
        {
            DateTime dt = new DateTime(year, month, day);
            _dayNumber = (int)(dt.Ticks / TimeSpan.TicksPerDay);
        }

        public int Year
        {
            get { return ToDateTimeAtMidnight().Year; }
        }

        public int Month
        {
            get { return ToDateTimeAtMidnight().Month; }
        }

        public int Day
        {
            get { return ToDateTimeAtMidnight().Day; }
        }

        public int DayOfYear
        {
            get { return ToDateTimeAtMidnight().DayOfYear; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return ToDateTimeAtMidnight().DayOfWeek; }
        }

        public static Date MinValue
        {
            get { return new Date(MinDayNumber); }
        }

        public static Date MaxValue
        {
            get { return new Date(MaxDayNumber); }
        }

        public int DayNumber
        {
            get { return _dayNumber; }
        }

        public DateTime At(TimeOfDay time)
        {
            long ticks = _dayNumber * TimeSpan.TicksPerDay + time.Ticks;
            return new DateTime(ticks);
        }

        public DateTime ToDateTimeAtMidnight()
        {
            return new DateTime(_dayNumber * TimeSpan.TicksPerDay);
        }

        public static Date Today(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utcNow, timeZone);
            return new Date(localNow.Year, localNow.Month, localNow.Day);
        }

        public static Date TodayLocal()
        {
            DateTime localNow = DateTime.Now;
            return new Date(localNow.Year, localNow.Month, localNow.Day);
        }

        public static Date TodayUtc()
        {
            DateTime utcNow = DateTime.UtcNow;
            return new Date(utcNow.Year, utcNow.Month, utcNow.Day);
        }

        public Date AddYears(int years)
        {
            DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
            DateTime result = dt.AddYears(years);
            return DateFromDateTime(result);
        }

        public Date AddMonths(int months)
        {
            DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
            DateTime result = dt.AddMonths(months);
            return DateFromDateTime(result);
        }

        public Date AddDays(int days)
        {
            DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
            DateTime result = dt.AddDays(days);
            return DateFromDateTime(result);
        }

        public Date SubtractYears(int years)
        {
            return AddYears(-years);
        }

        public Date SubtractMonths(int months)
        {
            return AddMonths(-months);
        }

        public Date SubtractDays(int days)
        {
            return AddDays(-days);
        }

        public bool Equals(Date other)
        {
            return _dayNumber == other._dayNumber;
        }

        public static int Compare(Date d1, Date d2)
        {
            if (d1._dayNumber > d2._dayNumber) return 1;
            if (d1._dayNumber < d2._dayNumber) return -1;
            return 0;
        }

        public int CompareTo(Date other)
        {
            return Compare(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Date && Equals((Date)obj);
        }

        public override int GetHashCode()
        {
            return _dayNumber;
        }

        public override string ToString()
        {
            return ToDateTimeAtMidnight().ToString("d");
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToDateTimeAtMidnight().ToString("d", formatProvider);
        }

        public string ToString(string format)
        {
            return ToDateTimeAtMidnight().ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToDateTimeAtMidnight().ToString(format, formatProvider);
        }

        public static Date Parse(string s)
        {
            DateTime dt = DateTime.Parse(s);
            return DateFromDateTime(dt);
        }

        public static Date Parse(string s, IFormatProvider provider)
        {
            DateTime dt = DateTime.Parse(s, provider);
            return DateFromDateTime(dt);
        }

        public static Date ParseExact(string s, string format, IFormatProvider provider)
        {
            DateTime dt = DateTime.ParseExact(s, format, provider);
            return DateFromDateTime(dt);
        }

        public static Date ParseExact(string s, string[] formats, IFormatProvider provider)
        {
            DateTime dt = DateTime.ParseExact(s, formats, provider, DateTimeStyles.None);
            return DateFromDateTime(dt);
        }

        public static bool TryParse(string s, out Date date)
        {
            DateTime dt;
            if (!DateTime.TryParse(s, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        public static bool TryParse(string s, IFormatProvider provider, out Date date)
        {
            DateTime dt;
            if (!DateTime.TryParse(s, provider, DateTimeStyles.None, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        public static bool TryParseExact(string s, string format, IFormatProvider provider, out Date date)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(s, format, provider, DateTimeStyles.None, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, out Date date)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(s, formats, provider, DateTimeStyles.None, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is Date))
                throw new ArgumentException();

            return Compare(this, (Date)obj);
        }

        public static bool operator ==(Date left, Date right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !left.Equals(right);
        }

        public static bool operator >(Date left, Date right)
        {
            return left._dayNumber > right._dayNumber;
        }

        public static bool operator >=(Date left, Date right)
        {
            return left._dayNumber >= right._dayNumber;
        }

        public static bool operator <(Date left, Date right)
        {
            return left._dayNumber < right._dayNumber;
        }

        public static bool operator <=(Date left, Date right)
        {
            return left._dayNumber <= right._dayNumber;
        }

        public static implicit operator Date(DateTime dateTime)
        {
            // This is useful such that existing items like DateTime.Today and DateTime.Date can be assigned to a Date type.

            return DateFromDateTime(dateTime);
        }

        private static Date DateFromDateTime(DateTime dateTime)
        {
            return new Date((int)(dateTime.Date.Ticks / TimeSpan.TicksPerDay));
        }
    }
}
