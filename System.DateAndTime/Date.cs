namespace System
{
    using System.Diagnostics.Contracts;

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
            get
            {
                DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
                return dt.Year;
            }
        }

        public int Month
        {
            get
            {
                DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
                return dt.Month;
            }
        }

        public int Day
        {
            get
            {
                DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
                return dt.Day;
            }
        }

        public int DayOfYear
        {
            get
            {
                DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
                return dt.DayOfYear;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
                return dt.DayOfWeek;
            }
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
            int dayNumber = (int)(result.Ticks / TimeSpan.TicksPerDay);
            return new Date(dayNumber);
        }

        public Date AddMonths(int months)
        {
            DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
            DateTime result = dt.AddMonths(months);
            int dayNumber = (int)(result.Ticks / TimeSpan.TicksPerDay);
            return new Date(dayNumber);
        }

        public Date AddDays(int days)
        {
            DateTime dt = new DateTime(_dayNumber * TimeSpan.TicksPerDay);
            DateTime result = dt.AddDays(days);
            int dayNumber = (int)(result.Ticks / TimeSpan.TicksPerDay);
            return new Date(dayNumber);
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

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is Date))
                throw new ArgumentException();

            return Compare(this, (Date) obj);
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
            
            return new Date((int)(dateTime.Date.Ticks / TimeSpan.TicksPerDay));
        }
    }
}
