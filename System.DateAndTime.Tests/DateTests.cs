using System.Globalization;
using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateTests
    {
        public DateTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [Fact]
        public void CanConstructDefaultDate()
        {
            Date date = new Date();
            Assert.Equal(0, date.DayNumber);
        }

        [Fact]
        public void CanConstructDateFromDayNumber()
        {
            Date date = new Date(3652058);
            Assert.Equal(3652058, date.DayNumber);
        }

        [Fact]
        public void CanConstructDateFromParts()
        {
            Date date = new Date(9999, 12, 31);
            Assert.Equal(3652058, date.DayNumber);
        }

        [Fact]
        public void CanGetYearFromDate()
        {
            Date date = new Date(2015, 12, 31);
            Assert.Equal(2015, date.Year);
        }

        [Fact]
        public void CanGetMonthFromDate()
        {
            Date date = new Date(2015, 12, 31);
            Assert.Equal(12, date.Month);
        }

        [Fact]
        public void CanGetDayFromDate()
        {
            Date date = new Date(2015, 12, 31);
            Assert.Equal(31, date.Day);
        }

        [Fact]
        public void CanGetDayOfWeekFromDate()
        {
            Date date = new Date(2015, 12, 31);
            Assert.Equal(DayOfWeek.Thursday, date.DayOfWeek);
        }

        [Fact]
        public void CanGetDayOfYearFromDate_StandardYear()
        {
            Date date = new Date(2015, 12, 31);
            Assert.Equal(365, date.DayOfYear);
        }

        [Fact]
        public void CanGetDayOfYearFromDate_LeapYear()
        {
            Date date = new Date(2000, 12, 31);
            Assert.Equal(366, date.DayOfYear);
        }

        [Fact]
        public void CanGetDateTimeFromDateAtTime()
        {
            Date date = new Date(2000, 12, 31);
            TimeOfDay time = new TimeOfDay(23, 59, 59);
            DateTime dt = date.At(time);

            DateTime expected = new DateTime(2000, 12, 31, 23, 59, 59);
            Assert.Equal(expected, dt);
        }

        [Fact]
        public void ToLongDateString()
        {
            var date = new Date(2000, 12, 31);
            var longDateString = date.ToLongDateString();
            Assert.Equal(longDateString, "Sunday, December 31, 2000");
        }

        [Fact]
        public void ToShortDateString()
        {
            var date = new Date(2000, 12, 31);
            var shortDateString = date.ToShortDateString();
            Assert.Equal(shortDateString, "12/31/2000");
        }

        [Fact]
        public void ToIsoString()
        {
            var date = new Date(2000, 12, 31);
            var isoString = date.ToIsoString();
            Assert.Equal(isoString, "2000-12-31");
        }
    }
}
