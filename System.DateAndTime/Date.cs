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

        /// <summary>
        /// Represents the smallest possible value of Date. This field is read-only.
        /// </summary>
        public static readonly Date MinValue = new Date(MinDayNumber);

        /// <summary>
        /// Represents the largest possible value of Date. This field is read-only.
        /// </summary>
        public static readonly Date MaxValue = new Date(MaxDayNumber);

        // Number of days in a non-leap year
        private const int DaysPerYear = 365;
        // Number of days in 4 years
        private const int DaysPer4Years = DaysPerYear * 4 + 1;       // 1461
        // Number of days in 100 years
        private const int DaysPer100Years = DaysPer4Years * 25 - 1;  // 36524
        // Number of days in 400 years
        private const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097

        // Number of days from 1/1/0001 to 12/31/1600
        private const int DaysTo1601 = DaysPer400Years * 4;          // 584388
        // Number of days from 1/1/0001 to 12/30/1899
        private const int DaysTo1899 = DaysPer400Years * 4 + DaysPer100Years * 3 - 367;
        // Number of days from 1/1/0001 to 12/31/1969
        internal const int DaysTo1970 = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear; // 719,162
        // Number of days from 1/1/0001 to 12/31/9999
        private const int DaysTo10000 = DaysPer400Years * 25 - 366;  // 3652059

        // The following arrays contain the starting day-of-year number of each month, for regular and leap years
        private static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        private static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };

        // internal enum
        private const int DatePartYear = 0;
        private const int DatePartDayOfYear = 1;
        private const int DatePartMonth = 2;
        private const int DatePartDay = 3;

        // Number of whole days since 0001-01-01 (which is day 0)
        private readonly int _dayNumber;

        /// <summary>
        /// Initializes a new instance of Date structure to a specified number of days.
        /// </summary>
        /// <param name="dayNumber">The number of days since January 1, 0001 in the
        /// proleptic Gregorian calendar.</param>
        public Date(int dayNumber)
        {
            if (dayNumber < MinDayNumber || dayNumber > MaxDayNumber)
                throw new ArgumentOutOfRangeException("dayNumber");
            Contract.EndContractBlock();

            _dayNumber = dayNumber;
        }

        /// <summary>
        /// Initializes a new instance of Date structure to a specified year, month,
        /// and day.
        /// </summary>
        /// <param name="year">The year (1 through 9999).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day (1 through the number of days in month).</param>
        /// <exception cref="ArgumentOutOfRangeException">year is less than 1 or greater than 9999.
        /// -or-
        /// month is less than 1 or greater than 12.
        /// -or-
        /// day is less than 1 or greater than the number of days in month.</exception>
        public Date(int year, int month, int day)
        {
            _dayNumber = DateToDayNumber(year, month, day);
        }

        public Date(int year, int dayOfYear)
        {
            if (dayOfYear < 1 || dayOfYear > (IsLeapYear(year) ? 366 : 365))
                throw new ArgumentOutOfRangeException("dayOfYear");
            Contract.EndContractBlock();

            _dayNumber = DateToDayNumber(year, 1, 1) + dayOfYear - 1;
        }

        /// <summary>
        /// Gets the year component of the date represented by this instance.
        /// </summary>
        public int Year
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 9999);
                return GetDatePart(DatePartYear);
            }
        }

        /// <summary>
        /// Gets the month component of the date represented by this instance.
        /// </summary>
        public int Month
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 12);
                return GetDatePart(DatePartMonth);
            }
        }

        /// <summary>
        /// Gets the day component of the date represented by this instance.
        /// </summary>
        public int Day
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 31);
                return GetDatePart(DatePartDay);
            }
        }

        /// <summary>
        /// Gets the day of the year represented by this instance.
        /// </summary>
        public int DayOfYear
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                Contract.Ensures(Contract.Result<int>() <= 366);
                return GetDatePart(DatePartDayOfYear);
            }
        }

        /// <summary>
        /// Gets the day of the week represented by this instance.
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get
            {
                Contract.Ensures(Contract.Result<DayOfWeek>() >= DayOfWeek.Sunday);
                Contract.Ensures(Contract.Result<DayOfWeek>() <= DayOfWeek.Saturday);
                return (DayOfWeek)((_dayNumber + 1) % 7);
            }
        }

        /// <summary>
        /// Gets the number of days since January 1, 0001 in the proleptic Gregorian calendar.
        /// </summary>
        public int DayNumber
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= MinDayNumber);
                Contract.Ensures(Contract.Result<int>() <= MaxDayNumber);
                return _dayNumber;
            }
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

        /// <summary>
        /// Returns an indication whether the specified year is a leap year.
        /// </summary>
        /// <param name="year">A 4-digit year.</param>
        /// <returns><c>true</c> if year is a leap year; otherwise, <c>false</c>.</returns>
        public static bool IsLeapYear(int year)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentOutOfRangeException("year");
            Contract.EndContractBlock();

            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        /// <summary>
        /// Returns the number of days in the month given by the year and month arguments.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>The number of days.</returns>
        public static int DaysInMonth(int year, int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("month");
            Contract.EndContractBlock();

            // IsLeapYear checks the year argument
            int[] days = IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
            return days[month] - days[month - 1];
        }

        public static Date Today(TimeZoneInfo timeZone)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utcNow, timeZone);
            return DateFromDateTime(localNow.Date);
        }

        public static Date TodayLocal()
        {
            DateTime localNow = DateTime.Now;
            return DateFromDateTime(localNow);
        }

        public static Date TodayUtc()
        {
            DateTime utcNow = DateTime.UtcNow;
            return DateFromDateTime(utcNow);
        }

        public Date AddYears(int years)
        {
            if (years < -10000 || years > 10000)
                throw new ArgumentOutOfRangeException("years");
            Contract.EndContractBlock();

            return AddMonths(years * 12);
        }

        public Date AddMonths(int months)
        {
            if (months < -120000 || months > 120000)
                throw new ArgumentOutOfRangeException("months");
            Contract.EndContractBlock();

            int y = GetDatePart(DatePartYear);
            int m = GetDatePart(DatePartMonth);
            int d = GetDatePart(DatePartDay);
            int i = m - 1 + months;

            if (i >= 0)
            {
                m = i % 12 + 1;
                y = y + i / 12;
            }
            else
            {
                m = 12 + (i + 1) % 12;
                y = y + (i - 11) / 12;
            }

            if (y < 1 || y > 9999)
                throw new ArgumentOutOfRangeException("months");

            int days = DaysInMonth(y, m);
            if (d > days) d = days;

            var dayNumber = DateToDayNumber(y, m, d);
            return new Date(dayNumber);
        }

        public Date AddDays(int days)
        {
            int dayNumber = _dayNumber + days;
            if (dayNumber < MinDayNumber || dayNumber > MaxDayNumber)
                throw new ArgumentOutOfRangeException("days");
            Contract.EndContractBlock();

            return new Date(dayNumber);
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

        /// <summary>
        /// Returns the day number count corresponding to the given year, month, and day.
        /// Will check the if the parameters are valid.
        /// </summary>
        private static int DateToDayNumber(int year, int month, int day)
        {
            if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
            {
                int[] days = IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
                if (day >= 1 && day <= days[month] - days[month - 1])
                {
                    int y = year - 1;
                    int n = y * 365 + y / 4 - y / 100 + y / 400 + days[month - 1] + day - 1;
                    return n;
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        // Returns a given date part of this DateTime. This method is used
        // to compute the year, day-of-year, month, or day part.
        private int GetDatePart(int part)
        {
            // n = number of days since 1/1/0001
            int n = _dayNumber;

            // y400 = number of whole 400-year periods since 1/1/0001
            int y400 = n / DaysPer400Years;

            // n = day number within 400-year period
            n -= y400 * DaysPer400Years;

            // y100 = number of whole 100-year periods within 400-year period
            int y100 = n / DaysPer100Years;

            // Last 100-year period has an extra day, so decrement result if 4
            if (y100 == 4) y100 = 3;

            // n = day number within 100-year period
            n -= y100 * DaysPer100Years;

            // y4 = number of whole 4-year periods within 100-year period
            int y4 = n / DaysPer4Years;

            // n = day number within 4-year period
            n -= y4 * DaysPer4Years;

            // y1 = number of whole years within 4-year period
            int y1 = n / DaysPerYear;

            // Last year has an extra day, so decrement result if 4
            if (y1 == 4) y1 = 3;

            // If year was requested, compute and return it
            if (part == DatePartYear)
            {
                return y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;
            }

            // n = day number within year
            n -= y1 * DaysPerYear;

            // If day-of-year was requested, return it
            if (part == DatePartDayOfYear) return n + 1;

            // Leap year calculation looks different from IsLeapYear since y1, y4,
            // and y100 are relative to year 1, not year 0
            bool leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
            int[] days = leapYear ? DaysToMonth366 : DaysToMonth365;

            // All months have less than 32 days, so n >> 5 is a good conservative
            // estimate for the month
            int m = n >> 5 + 1;

            // m = 1-based month number
            while (n >= days[m]) m++;

            // If month was requested, return it
            if (part == DatePartMonth) return m;

            // Return 1-based day-of-month
            return n - days[m - 1] + 1;
        }
    }
}
