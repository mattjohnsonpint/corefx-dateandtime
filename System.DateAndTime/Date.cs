﻿using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System
{
    /// <summary>
    /// Represents a whole date, having a year, month and day component.
    /// All values are in the proleptic Gregorian (ISO 8601) calendar system unless otherwise specified.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    [XmlSchemaProvider("GetSchema")]
    public struct Date : IEquatable<Date>, IComparable<Date>, IComparable, IFormattable, IXmlSerializable
    {
        private const int MinDayNumber = 0;
        private const int MaxDayNumber = 3652058;

        /// <summary>
        /// Represents the smallest possible value of <see cref="Date"/>. This field is read-only.
        /// </summary>
        public static readonly Date MinValue = new Date(MinDayNumber);

        /// <summary>
        /// Represents the largest possible value of <see cref="Date"/>. This field is read-only.
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

        // The following arrays contain the starting day-of-year number of each month, for regular and leap years
        private static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        private static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };

        // internal enum
        private const int DatePartYear = 0;
        private const int DatePartDayOfYear = 1;
        private const int DatePartMonth = 2;
        private const int DatePartDay = 3;

        // Number of whole days since 0001-01-01 (which is day 0)
        // NOTE: This is the only field in this structure.
        private readonly int _dayNumber;

        /// <summary>
        /// Initializes a new instance of a <see cref="Date"/> structure to a specified number of days.
        /// </summary>
        /// <param name="dayNumber">The number of days since January 1, 0001 in the proleptic Gregorian calendar.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="dayNumber"/> is out of the range supported by the <see cref="Date"/> object.
        /// </exception>
        public Date(int dayNumber)
        {
            if (dayNumber < MinDayNumber || dayNumber > MaxDayNumber)
                throw new ArgumentOutOfRangeException("dayNumber");
            Contract.EndContractBlock();

            _dayNumber = dayNumber;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Date"/> structure to a specified year, month, and day.
        /// </summary>
        /// <param name="year">The year (1 through 9999).</param>
        /// <param name="month">The month (1 through 12).</param>
        /// <param name="day">The day (1 through the number of days in <paramref name="month"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is less than 1 or greater than 9999.
        /// <para>-or-</para>
        /// <paramref name="month"/> is less than 1 or greater than 12.
        /// <para>-or-</para>
        /// <paramref name="day"/> is less than 1 or greater than the number of days in <paramref name="month"/>.
        /// </exception>
        public Date(int year, int month, int day)
        {
            _dayNumber = DateToDayNumber(year, month, day);
        }

        /// <summary>
        /// Initializes a new instance of Date structure to a specified year, month, and day for the specified calendar.
        /// </summary>
        /// <param name="year">The year (1 through the number of years in <paramref name="calendar"/>).</param>
        /// <param name="month">The month (1 through the number of months in <paramref name="calendar"/>).</param>
        /// <param name="day">The day (1 through the number of days in <paramref name="month"/>).</param>
        /// <param name="calendar">
        /// The calendar that is used to interpret <paramref name="year"/>,
        /// <paramref name="month"/>, and <paramref name="day"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="calendar"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is not in the range supported by <paramref name="calendar"/>.
        /// <para>-or-</para>
        /// <paramref name="month"/> is less than 1 or greater than the number of months in <paramref name="calendar"/>.
        /// <para>-or-</para>
        /// <paramref name="day"/> is less than 1 or greater than the number of days in <paramref name="month"/>.
        /// </exception>
        public Date(int year, int month, int day, Calendar calendar)
        {
            DateTime dt = calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            _dayNumber = (int)(dt.Ticks / TimeSpan.TicksPerDay);
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Date"/> structure to a specified year, and day of year.
        /// </summary>
        /// <param name="year">The year (1 through 9999).</param>
        /// <param name="dayOfYear">The day of year (1 through the number of days in <paramref name="year"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is less than 1 or greater than 9999.
        /// <para>-or-</para>
        /// <paramref name="dayOfYear"/> is less than 1 or greater than the number of days in <paramref name="year"/>.
        /// </exception>
        /// <remarks>
        /// Note that standard years have days numbered 1 through 365, while leap years have days numbered 1 through 366.
        /// </remarks>
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

        /// <summary>
        /// Creates a <see cref="DateTime"/> object from the current <see cref="Date"/> and the specified <see cref="TimeOfDay"/>.
        /// The resulting value has a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <remarks>
        /// Since neither <see cref="Date"/> or <see cref="TimeOfDay"/> keep track of <see cref="DateTimeKind"/>,
        /// recognize that the <see cref="DateTime"/> produced by <c>Date.Now.At(TimeOfDay.Now)</c> will have
        /// <see cref="DateTimeKind.Unspecified"/>, rather than then <see cref="DateTimeKind.Local"/> that would be
        /// given by <c>DateTime.Now</c>.
        /// <para>The same applies for <see cref="DateTimeKind.Utc"/>.</para>
        /// </remarks>
        public DateTime At(TimeOfDay time)
        {
            long ticks = _dayNumber * TimeSpan.TicksPerDay + time.Ticks;
            return new DateTime(ticks);
        }

        /// <summary>
        /// Creates a <see cref="DateTime"/> object from the current <see cref="Date"/>, with the time set to midnight
        /// (00:00:00). The resulting value has a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Unspecified"/>.
        /// </summary>
        /// <remarks>
        /// Note that midnight might be ambiguous or invalid in some time zones on DST transition days.
        /// Though this method is time zone ignorant, the resulting value should be considered suspect and used with
        /// caution.
        /// </remarks>
        public DateTime ToDateTimeAtMidnight()
        {
            return new DateTime(_dayNumber * TimeSpan.TicksPerDay);
        }

        /// <summary>
        /// Returns an indication whether the specified year is a leap year.
        /// </summary>
        /// <param name="year">A 4-digit year.</param>
        /// <returns><c>true</c> if year is a leap year; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is less than 1 or greater than 9999.
        /// </exception>
        public static bool IsLeapYear(int year)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentOutOfRangeException("year");
            Contract.EndContractBlock();

            return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
        }

        /// <summary>
        /// Returns the number of days in the specified month and year.
        /// </summary>
        /// <returns>
        /// The number of days in <paramref name="month"/> for the specified <paramref name="year"/>.
        /// For example, if <paramref name="month"/> equals 2 for February, the return value is 28 or 29 depending
        /// upon whether <paramref name="year"/> is a leap year.
        /// </returns>
        /// <param name="year">The year.</param>
        /// <param name="month">The month (a number ranging from 1 to 12).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="month"/> is less than 1 or greater than 12.
        /// <para>-or-</para>
        /// <paramref name="year"/> is less than 1 or greater than 9999.
        /// </exception>
        public static int DaysInMonth(int year, int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("month");
            Contract.EndContractBlock();

            // IsLeapYear checks the year argument
            int[] days = IsLeapYear(year) ? DaysToMonth366 : DaysToMonth365;
            return days[month] - days[month - 1];
        }

        /// <summary>
        /// Gets a <see cref="Date"/> object that is set to the current date in the specified time zone.
        /// </summary>
        /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/> instance.</param>
        /// <returns>The current <see cref="Date"/> for the specified time zone.</returns>
        public static Date TodayInTimeZone(TimeZoneInfo timeZoneInfo)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo);
            return DateFromDateTime(localNow.Date);
        }

        /// <summary>
        /// Gets the current date in the local time zone.
        /// </summary>
        public static Date Today
        {
            get
            {
                DateTime localNow = DateTime.Now;
                return DateFromDateTime(localNow);
            }
        }

        /// <summary>
        /// Gets the current date in Coordinated Universal Time (UTC).
        /// </summary>
        public static Date UtcToday
        {
            get
            {
                DateTime utcNow = DateTime.UtcNow;
                return DateFromDateTime(utcNow);
            }
        }

        /// <summary>
        /// Gets a <see cref="Date"/> object whose value is ahead or behind the value of this instance by the specified
        /// number of years. Positive values will move the date forward; negative values will move the date backwards.
        /// <para>
        /// If the original date is a leap day (February 29), and the resulting year is not a leap year, the resulting
        /// value will be adjusted to February 28.
        /// </para>
        /// </summary>
        /// <returns>
        /// A new <see cref="Date"/> object which is the result of adjusting this instance by the
        /// <paramref name="years"/> specified.
        /// </returns>
        /// <param name="years">The number of years to adjust by. The value can be negative or positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="years"/> or the resulting <see cref="Date"/> is less than <see cref="Date.MinValue"/>
        /// or greater than <see cref="Date.MaxValue"/>.
        /// </exception>
        public Date AddYears(int years)
        {
            if (years < -10000 || years > 10000)
                throw new ArgumentOutOfRangeException("years");
            Contract.EndContractBlock();

            return AddMonths(years * 12);
        }

        /// <summary>
        /// Gets a <see cref="Date"/> object whose value is ahead or behind the value of this instance by the specified
        /// number of months. Positive values will move the date forward; negative values will move the date backwards.
        /// <para>
        /// Since the number of days in a months varies, the resulting date may not necessarily fall on the same
        /// day. If the resulting value would have landed on a day that doesn't exist within a month, the value is
        /// adjusted backward to the last day of the month.
        /// </para>
        /// </summary>
        /// <returns>
        /// A new <see cref="Date"/> object which is the result of adjusting this instance by the
        /// <paramref name="months"/> specified.
        /// </returns>
        /// <param name="months">The number of months to adjust by. The value can be negative or positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="months"/> or the resulting <see cref="Date"/> is less than <see cref="Date.MinValue"/>
        /// or greater than <see cref="Date.MaxValue"/>.
        /// </exception>
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

        /// <summary>
        /// Gets a <see cref="Date"/> object whose value is ahead or behind the value of this instance by the specified
        /// number of days. Positive values will move the date forward; negative values will move the date backwards.
        /// </summary>
        /// <returns>
        /// A new <see cref="Date"/> object which is the result of adjusting this instance by the
        /// <paramref name="days"/> specified.
        /// </returns>
        /// <param name="days">The number of days to adjust by. The value can be negative or positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="days"/> or the resulting <see cref="Date"/> is less than <see cref="Date.MinValue"/>
        /// or greater than <see cref="Date.MaxValue"/>.
        /// </exception>
        public Date AddDays(int days)
        {
            int dayNumber = _dayNumber + days;
            if (dayNumber < MinDayNumber || dayNumber > MaxDayNumber)
                throw new ArgumentOutOfRangeException("days");
            Contract.EndContractBlock();

            return new Date(dayNumber);
        }

        /// <summary>
        /// Returns the number of days remaining from this date to the <paramref name="date"/> specified.
        /// If the <paramref name="date"/> has already passed, the result will be negative.
        /// </summary>
        /// <param name="date">The target <see cref="Date"/> value.</param>
        /// <returns>The integer number of days until the date specified.</returns>
        /// <remarks>
        /// Unlike the <see cref="DaysInRange"/> method, this operation uses an exclusive upper bound.
        /// For example, if the current instance represents January 1st, there is one day until January 2nd.
        /// </remarks>
        public int DaysUntil(Date date)
        {
            return date.DayNumber - _dayNumber;
        }

        /// <summary>
        /// Returns the number of days in the range of the two dates specified.  The parameters <paramref name="d1"/>
        /// and <paramref name="d2"/> make up a fully-inclusive range of dates.  For example, there are
        /// two days between January 1st and January 2nd.
        /// <para>The order of the parameters does not matter; the result is always positive.</para>
        /// </summary>
        /// <param name="d1">The first <see cref="Date"/> in the inclusive range.</param>
        /// <param name="d2">The second <see cref="Date"/> in the inclusive range.</param>
        /// <returns>The number of days in the range.</returns>
        /// <remarks>
        /// Because a <see cref="Date"/> inherently represents a whole day, the range used by this method is
        /// fully-inclusive.  This is unlike the <see cref="MonthsInRange"/> and <see cref="YearsInRange"/> methods.
        /// <seealso cref="DaysUntil"/>
        /// </remarks>
        public static int DaysInRange(Date d1, Date d2)
        {
            return Math.Abs(d2.DayNumber - d1.DayNumber) + 1;
        }

        /// <summary>
        /// Returns the number of whole months in the range of the two dates specified.
        /// The parameters <paramref name="d1"/> and <paramref name="d2"/> make up a half-open range of dates.
        /// For example, there is one whole month between January 1st and February 1st.
        /// <para>The order of the parameters does not matter; the result is always positive.</para>
        /// <para>The calculation does not include partial months.</para>
        /// </summary>
        /// <param name="d1">The first <see cref="Date"/> in the range.</param>
        /// <param name="d2">The second <see cref="Date"/> in the range.</param>
        /// <returns>The number of whole months in the range.</returns>
        /// <remarks>
        /// Note that the calculation assumes the proleptic Gregorian calendar.
        /// </remarks>
        public static int MonthsInRange(Date d1, Date d2)
        {
            if (d1 == d2)
                return 0;

            Date min = d1 < d2 ? d1 : d2;
            Date max = d1 > d2 ? d1 : d2;

            int months = (max.Year * 12 + max.Month) - (min.Year * 12 + min.Month);
            if (min > max.AddMonths(-months))
                months--;

            return months;
        }

        /// <summary>
        /// Returns the number of whole years in the range of the two dates specified.
        /// The parameters <paramref name="d1"/> and <paramref name="d2"/> make up a half-open range of dates.
        /// For example, there is one whole year between <c>2000-01-01</c> and <c>2001-01-01</c>.
        /// <para>The order of the parameters does not matter; the result is always positive.</para>
        /// <para>The calculation does not include partial years.</para>
        /// </summary>
        /// <param name="d1">The first <see cref="Date"/> in the range.</param>
        /// <param name="d2">The second <see cref="Date"/> in the range.</param>
        /// <returns>The number of whole years in the range.</returns>
        /// <remarks>
        /// This method is useful for calculating a person's age, and similar anniversaries.
        /// </remarks>
        public static int YearsInRange(Date d1, Date d2)
        {
            if (d1 == d2)
                return 0;

            Date min = d1 < d2 ? d1 : d2;
            Date max = d1 > d2 ? d1 : d2;

            int years = max.Year - min.Year;
            if (min > max.AddYears(-years))
                years--;

            return years;
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified
        /// <see cref="Date"/> instance.
        /// </summary>
        /// <param name="value">The other date object to compare against this instance.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> parameter equals the value of this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Date value)
        {
            return _dayNumber == value._dayNumber;
        }

        /// <summary>
        /// Returns a value indicating whether two <see cref="Date"/> instances have the same date value.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns><c>true</c> if the two values are equal; otherwise, <c>false</c>.</returns>
        public static bool Equals(Date d1, Date d2)
        {
            return d1.Equals(d2);
        }

        /// <summary>
        /// Compares two instances of <see cref="Date"/> and returns an integer that indicates whether the first
        /// instance is earlier than, the same as, or later than the second instance.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of <paramref name="d1"/> and <paramref name="d2"/>.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item>
        ///   <term>Less than zero</term>
        ///   <term><paramref name="d1"/> is earlier than <paramref name="d2"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Zero</term>
        ///   <term><paramref name="d1"/> is the same as <paramref name="d2"/>.</term>
        /// </item>
        /// <item>
        ///   <term>Greater than zero</term>
        ///   <term><paramref name="d1"/> is later than <paramref name="d2"/>.</term>
        /// </item>
        /// </list>
        /// </returns>
        public static int Compare(Date d1, Date d2)
        {
            if (d1._dayNumber > d2._dayNumber) return 1;
            if (d1._dayNumber < d2._dayNumber) return -1;
            return 0;
        }

        /// <summary>
        /// Compares the value of this instance to a specified <see cref="Date"/> value and returns an integer that
        /// indicates whether this instance is earlier than, the same as, or later than the specified
        /// <see cref="Date"/> value.
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
        public int CompareTo(Date value)
        {
            return Compare(this, value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is an instance of <see cref="Date"/>
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (ReferenceEquals(null, value)) return false;
            return value is Date && Equals((Date)value);
        }

        /// <summary>
        /// Returns the hash code of this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <remarks>
        /// The hash code of a <see cref="Date"/> object is the day number, which is the 
        /// number of days since January 1, 0001 in the proleptic Gregorian calendar.
        /// </remarks>
        public override int GetHashCode()
        {
            return _dayNumber;
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of value of the current <see cref="Date"/> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The date is outside the range of dates supported by the calendar used by the current culture.
        /// </exception>
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return ToDateTimeAtMidnight().ToString("d");
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent string representation
        /// using the specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A string representation of value of the current <see cref="Date"/> object as specified by
        /// <paramref name="provider"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The date is outside the range of dates supported by the calendar used by <paramref name="provider"/>.
        /// </exception>
        public string ToString(IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return ToDateTimeAtMidnight().ToString("d", provider);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent string representation
        /// using the specified format.
        /// </summary>
        /// <param name="format">A standard or custom date format string.</param>
        /// <returns>
        /// A string representation of value of the current <see cref="Date"/> object as specified by
        /// <paramref name="format"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// The length of <paramref name="format"/> is 1, and it is not one of the format specifier characters defined
        /// for <see cref="DateTimeFormatInfo"/>.
        /// <para>-or-</para>
        /// <paramref name="format"/> does not contain a valid custom format pattern.
        /// <para>-or-</para>
        /// The standard or custom format specified is not valid for a <see cref="Date"/> object, because it contains
        /// a time-of-day component.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The date is outside the range of dates supported by the calendar used by the current culture.
        /// </exception>
        public string ToString(string format)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            format = NormalizeDateFormat(format);

            return ToDateTimeAtMidnight().ToString(format);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom date format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// A string representation of value of the current <see cref="Date"/> object as specified by
        /// <paramref name="format"/> and <paramref name="provider"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// The length of <paramref name="format"/> is 1, and it is not one of the format specifier characters defined
        /// for <see cref="DateTimeFormatInfo"/>.
        /// <para>-or-</para>
        /// <paramref name="format"/> does not contain a valid custom format pattern.
        /// <para>-or-</para>
        /// The standard or custom format specified is not valid for a <see cref="Date"/> object, because it contains
        /// a time-of-day component.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The date is outside the range of dates supported by the calendar used by <paramref name="provider"/>.
        /// </exception>
        public string ToString(string format, IFormatProvider provider)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            format = NormalizeDateFormat(format);

            return ToDateTimeAtMidnight().ToString(format, provider);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent long date string
        /// representation.
        /// </summary>
        /// <returns>
        /// A string that contains the long date string representation of the current <see cref="Date"/> object.
        /// </returns>
        /// <remarks>
        /// The value of the current <see cref="Date"/> object is formatted using the pattern defined by the
        /// <see cref="DateTimeFormatInfo.LongDatePattern" /> property associated with the current thread culture.
        /// </remarks>
        public string ToLongDateString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent short date string
        /// representation.
        /// </summary>
        /// <returns>
        /// A string that contains the short date string representation of the current <see cref="Date"/> object.
        /// </returns>
        /// <remarks>
        /// The value of the current <see cref="Date"/> object is formatted using the pattern defined by the
        /// <see cref="DateTimeFormatInfo.ShortDatePattern" /> property associated with the current thread culture.
        /// </remarks>
        public string ToShortDateString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent ISO standard string
        /// representation (ISO-8601), which has the format: <c>yyyy-MM-dd</c>.
        /// </summary>
        /// <returns>
        /// A string that contains the ISO standard string representation of the current <see cref="Date"/> object.
        /// </returns>
        /// <remarks>
        /// Because the ISO-8601 standard uses the proleptic Gregorian calendar, this method always uses the calendar
        /// of the <see cref="CultureInfo.InvariantCulture"/>, despite the setting of the current culture.
        /// </remarks>
        public string ToIsoString()
        {
            return ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the string representation of a date to its <see cref="Date"/> equivalent.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <returns>An object that is equivalent to the date contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> does not contain a valid string representation of a date.
        /// </exception>
        public static Date Parse(string s)
        {
            DateTime dt = DateTime.Parse(s);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the string representation of a date to its <see cref="Date"/> equivalent
        /// by using culture-specific format information.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the date contained in <paramref name="s"/>,
        /// as specified by <paramref name="provider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> does not contain a valid string representation of a date.
        /// </exception>
        public static Date Parse(string s, IFormatProvider provider)
        {
            DateTime dt = DateTime.Parse(s, provider);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the string representation of a date to its <see cref="Date"/> equivalent
        /// by using culture-specific format information and formatting style.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the date contained in <paramref name="s"/>,
        /// as specified by <paramref name="provider"/> and <paramref name="styles"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> does not contain a valid string representation of a date.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static Date Parse(string s, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            DateTime dt = DateTime.Parse(s, provider, styles);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified format and culture-specific format information.
        /// The format of the string representation must match the specified format exactly or an exception is thrown.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <param name="format">A format specifier that defines the required format of <paramref name="s"/>.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the date contained in <paramref name="s"/>,
        /// as specified by <paramref name="format"/> and <paramref name="provider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> or <paramref name="format"/> is an empty string.
        /// <para>-or-</para>
        /// <paramref name="s"/> does not contain a date that corresponds to the pattern specified in <paramref name="format"/>.
        /// <para>-or-</para>
        /// <paramref name="format"/> contains a format pattern that is not applicable to a <see cref="Date"/>.
        /// </exception>
        public static Date ParseExact(string s, string format, IFormatProvider provider)
        {
            format = NormalizeDateFormat(format);

            DateTime dt = DateTime.ParseExact(s, format, provider);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified format, culture-specific format information, and style.
        /// The format of the string representation must match the specified format exactly or an exception is thrown.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <param name="format">A format specifier that defines the required format of <paramref name="s"/>.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the date contained in <paramref name="s"/>,
        /// as specified by <paramref name="format"/>, <paramref name="provider"/> and <paramref name="styles"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> or <paramref name="format"/> is an empty string.
        /// <para>-or-</para>
        /// <paramref name="s"/> does not contain a date that corresponds to the pattern specified in <paramref name="format"/>.
        /// <para>-or-</para>
        /// <paramref name="format"/> contains a format pattern that is not applicable to a <see cref="Date"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static Date ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            format = NormalizeDateFormat(format);

            DateTime dt = DateTime.ParseExact(s, format, provider, styles);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified array of formats, culture-specific format information, and style.
        /// The format of the string representation must match at least one of the specified formats
        /// exactly or an exception is thrown.
        /// </summary>
        /// <param name="s">A string that contains a date to convert.</param>
        /// <param name="formats">An array of allowable formats of <paramref name="s"/>.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the date contained in <paramref name="s"/>,
        /// as specified by <paramref name="formats"/>, <paramref name="provider"/> and <paramref name="styles"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is an empty string.
        /// <para>-or-</para>
        /// An element of <paramref name="formats"/> is an empty string.
        /// <para>-or-</para>
        /// <paramref name="s"/> does not contain a date that corresponds to any element of <paramref name="formats"/>.
        /// <para>-or-</para>
        /// An element of <paramref name="formats"/> contains a format pattern that is not applicable to a <see cref="Date"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static Date ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles styles)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            for (int i = 0; i < formats.Length; i++)
            {
                formats[i] = NormalizeDateFormat(formats[i]);
            }

            DateTime dt = DateTime.ParseExact(s, formats, provider, styles);
            return DateFromDateTime(dt);
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">A string containing a date to convert.</param>
        /// <param name="date">
        /// When this method returns, contains the <see cref="Date"/> value equivalent to the date
        /// contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="MinValue"/>
        /// if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is
        /// <c>null</c>, is an empty string (""), or does not contain a valid string representation of a date.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified culture-specific format information and formatting style,
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">A string containing a date to convert.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <param name="date">
        /// When this method returns, contains the <see cref="Date"/> value equivalent to the date
        /// contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="MinValue"/>
        /// if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is
        /// <c>null</c>, is an empty string (""), or does not contain a valid string representation of a date.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out Date date)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            DateTime dt;
            if (!DateTime.TryParse(s, provider, styles, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified format, culture-specific format information, and style.
        /// The format of the string representation must match the specified format exactly.
        /// The method returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">A string containing a date to convert.</param>
        /// <param name="format">A format specifier that defines the required format of <paramref name="s"/>.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <param name="date">
        /// When this method returns, contains the <see cref="Date"/> value equivalent to the date
        /// contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="MinValue"/>
        /// if the conversion failed. The conversion fails if either the <paramref name="s"/> or
        /// <paramref name="format"/> parameter is <c>null</c>, is an empty string (""), or does not
        /// contain a date that coresponds to the pattern specified in <paramref name="format"/>.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles styles, out Date date)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            format = NormalizeDateFormat(format);

            DateTime dt;
            if (!DateTime.TryParseExact(s, format, provider, styles, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        /// <summary>
        /// Converts the specified string representation of a date to its <see cref="Date"/> equivalent
        /// using the specified array of formats, culture-specific format information, and style.
        /// The format of the string representation must match at least one of the specified formats exactly.
        /// The method returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">A string containing a date to convert.</param>
        /// <param name="formats">An array of allowable formats of <paramref name="s"/>.</param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about <paramref name="s"/>.
        /// </param>
        /// <param name="styles">
        /// A bitwise combination of the enumeration values that indicates the style elements that
        /// can be present in <paramref name="s"/> for the parse operation to succeed.
        /// Note that only styles related to whitespace handling are applicable on a <see cref="Date"/>.
        /// A typical value to specify is <see cref="DateTimeStyles.None"/>.
        /// </param>
        /// <param name="date">
        /// When this method returns, contains the <see cref="Date"/> value equivalent to the date
        /// contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="MinValue"/>
        /// if the conversion failed. The conversion fails if either the <paramref name="s"/> or
        /// <paramref name="formats"/> parameter is <c>null</c>, <paramref name="s"/> or an element of
        /// <paramref name="formats"/> is an empty string (""), or the format of <paramref name="s"/> is not
        /// exactly as specified by at least one of the format patterns in <paramref name="formats"/>.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="s"/> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="styles"/> styles contains an invalid <see cref="DateTimeStyles"/> values.
        /// The only styles that are valid for a <see cref="Date"/> are:
        /// <see cref="DateTimeStyles.None"/>, <see cref="DateTimeStyles.AllowLeadingWhite"/>,
        /// <see cref="DateTimeStyles.AllowTrailingWhite"/>, <see cref="DateTimeStyles.AllowInnerWhite"/>, and
        /// <see cref="DateTimeStyles.AllowWhiteSpaces"/>.  The other styles are invalid because they only apply
        /// when both a date and time are being parsed together.
        /// </exception>
        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles styles, out Date date)
        {
            if (((int)styles) >= 8)
                throw new ArgumentException("styles");
            Contract.EndContractBlock();

            for (int i = 0; i < formats.Length; i++)
            {
                formats[i] = NormalizeDateFormat(formats[i]);
            }

            DateTime dt;
            if (!DateTime.TryParseExact(s, formats, provider, styles, out dt))
            {
                date = default(Date);
                return false;
            }

            date = DateFromDateTime(dt);
            return true;
        }

        /// <summary>
        /// Compares the value of this instance to a specified object that contains a <see cref="Date"/> value and
        /// returns an integer that indicates whether this instance is earlier than, the same as, or later than the
        /// specified <see cref="Date"/> value.
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
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (!(value is Date))
                throw new ArgumentException();

            return Compare(this, (Date)value);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Date"/> are equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> and <paramref name="d2"/> represent the same date;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Date d1, Date d2)
        {
            return d1.Equals(d2);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Date"/> are not equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> and <paramref name="d2"/> do not represent the same date;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Date d1, Date d2)
        {
            return !d1.Equals(d2);
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is later than another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> is later than <paramref name="d2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(Date d1, Date d2)
        {
            return d1._dayNumber > d2._dayNumber;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is equal to or later than another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> is equal to or later than <paramref name="d2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(Date d1, Date d2)
        {
            return d1._dayNumber >= d2._dayNumber;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is earlier than another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> is earlier than <paramref name="d2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(Date d1, Date d2)
        {
            return d1._dayNumber < d2._dayNumber;
        }

        /// <summary>
        /// Determines whether one specified <see cref="Date"/> is equal to or earlier than another specified <see cref="Date"/>.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="d1"/> is equal to or earlier than <paramref name="d2"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(Date d1, Date d2)
        {
            return d1._dayNumber <= d2._dayNumber;
        }

        /// <summary>
        /// Implicitly casts a <see cref="DateTime"/> object to a <see cref="Date"/> by returning a new
        /// <see cref="Date"/> object that has the equivalent year, month, and day components.  This is useful when
        /// using APIs that express a calendar date as a <see cref="DateTime"/> and expect the consumer to ignore
        /// the time portion of the value.  This operator enables these values to be assigned to a variable having
        /// a <see cref="Date"/> type.
        /// </summary>
        /// <param name="dateTime">A <see cref="DateTime"/> value whose date portion will be used to construct a new
        /// <see cref="Date"/> object, and whose time portion will be ignored.</param>
        /// <returns>A newly constructed <see cref="Date"/> object with an equivalent date value.</returns>
        /// <remarks>
        /// Fundamentally, a date-only value and a date-time value are two different concepts.  In previous versions
        /// of the .NET framework, the <see cref="Date"/> type did not exist, and thus several date-only values were
        /// represented by using a <see cref="DateTime"/> with the time set to midnight (00:00:00).  For example, the
        /// <see cref="DateTime.Today"/> and <see cref="DateTime.Date"/> properties exhibit this behavior.
        /// This implicit cast operator allows those APIs to be naturally used with <see cref="Date"/>.
        /// <para>
        /// Also note that when evaluated as a full date-time, the input <paramref name="dateTime"/> might not actually
        /// exist, since some time zones (ex: Brazil) will spring-forward directly from 23:59 to 01:00, skipping over
        /// midnight.  Using a <see cref="Date"/> object avoids this particular edge case, and several others.
        /// </para>
        /// </remarks>
        public static implicit operator Date(DateTime dateTime)
        {
            return DateFromDateTime(dateTime);
        }

        /// <summary>
        /// Implicitly casts a <see cref="Date"/> object to a <see cref="DateTime"/> by returning a new
        /// <see cref="DateTime"/> object that has the equivalent year, month, and day components, and has its time
        /// set to midnight (00:00:00).  This is useful when using APIs that express a calendar date as a
        /// <see cref="DateTime"/> and ignore the time portion of the value.  This operator enables <see cref="Date"/>
        /// values to be passed to a method expecting a <see cref="DateTime"/>.
        /// <para>
        /// Use with caution, as midnight may not necessarily be valid in every time zone on every day of the year.
        /// For example, when Brazil springs forward for daylight saving time, the clocks skip from 23:59:59 directly
        /// to 01:00:00.
        /// </para>
        /// </summary>
        /// <param name="date">A <see cref="Date"/> value whose date portion will be used to construct a new
        /// <see cref="DateTime"/> object.</param>
        /// <returns>
        /// A newly constructed <see cref="DateTime"/> object with an equivalent date value, and the time set
        /// to midnight (00:00:00).
        /// </returns>
        /// <remarks>
        /// Fundamentally, a date-only value and a date-time value are two different concepts.  In previous versions
        /// of the .NET framework, the <see cref="Date"/> type did not exist, and thus several date-only values were
        /// represented by using a <see cref="DateTime"/> with the time set to midnight (00:00:00).  For example, the
        /// <see cref="Calendar.GetYear"/> method expects a <see cref="DateTime"/>, even though it only uses the date
        /// component. This implicit cast operator allows those APIs to be naturally used with <see cref="Date"/>.
        /// </remarks>
        public static implicit operator DateTime(Date date)
        {
            return date.ToDateTimeAtMidnight();
        }

        /// <summary>
        /// Constructs a <see cref="Date"/> object from the date component of a <see cref="DateTime"/>.
        /// </summary>
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

        /// <summary>
        /// Returns a given date part of this DateTime. This method is used
        /// to compute the year, day-of-year, month, or day part.
        /// </summary>
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

        /// <summary>
        /// Normalizes a format string that has standard or custom date/time formats,
        /// such that the formatted output can only contain a date when applied.
        /// </summary>
        /// <exception cref="FormatException">
        /// The format string contained a format specifier that is only applicable
        /// when a time-of-day would be part of the formatted output.
        /// </exception>
        private static string NormalizeDateFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return "d";
            }

            // standard formats
            if (format.Length == 1)
            {
                // pass-through formats
                if ("DdMmYy".Contains(format))
                    return format;

                // ISO formats
                if ("Oos".Contains(format))
                    return "yyyy-MM-dd";

                // All other standard DateTime formats are invalid for Date
                throw new FormatException();
            }

            // custom format - test for time components or embedded standard time formats
            // except when escaped by preceding \ or enclosed in "" or '' quotes

            var filtered = Regex.Replace(format, @"(\\.)|("".*"")|('.*')", String.Empty);
            if (Regex.IsMatch(filtered, "([fFghHKmstz:]+)|(%[fFgGrRtTuU]+)"))
                throw new FormatException();

            // custom format with embedded standard format(s) - ISO replacement
            format = Regex.Replace(format, @"(%[Oos])", "yyyy-MM-dd");

            // pass through
            return format;
        }

        /// <summary>
        /// Gets a <see cref="XmlQualifiedName"/> that represents the <c>xs:date</c> type of the
        /// W3C XML Schema Definition (XSD) specification.
        /// </summary>
        /// <remarks>
        /// This is required to support the <see cref="XmlSchemaProviderAttribute"/> applied to this structure.
        /// </remarks>
        public static XmlQualifiedName GetSchema(object xs)
        {
            return new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema");
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
        /// Generates a <see cref="Date"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
        /// <remarks>
        /// An <c>xs:date</c> uses the ISO-8601 extended date format. The equivalent .NET Framework format string
        /// is <c>yyyy-MM-dd</c>.  This method always uses the proleptic Gregorian calendar of the
        /// <see cref="CultureInfo.InvariantCulture"/>, regardless of the current culture setting.
        /// </remarks>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var s = reader.NodeType == XmlNodeType.Element
                ? reader.ReadElementContentAsString()
                : reader.ReadContentAsString();

            Date d;
            if (!TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            {
                throw new FormatException();
            }

            this = d;
        }

        /// <summary>
        /// Converts a <see cref="Date"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
        /// <remarks>
        /// An <c>xs:date</c> uses the ISO-8601 extended date format. The equivalent .NET Framework format string
        /// is <c>yyyy-MM-dd</c>.  This method always uses the proleptic Gregorian calendar of the
        /// <see cref="CultureInfo.InvariantCulture"/>, regardless of the current culture setting.
        /// </remarks>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(ToIsoString());
        }
    }
}
