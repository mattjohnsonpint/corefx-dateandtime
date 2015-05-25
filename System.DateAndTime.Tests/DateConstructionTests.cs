using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateConstructionTests
    {
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
        public void CannotConstructDateFromDayNumberTooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(3652059));
        }

        [Fact]
        public void CannotConstructDateFromDayNumberTooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(-1));
        }

        [Fact]
        public void CanConstructDateFromParts()
        {
            Date date = new Date(9999, 12, 31);
            Assert.Equal(3652058, date.DayNumber);
        }

        [Fact]
        public void CanConstructDateFromYearAndDayOfYear_NonLeap()
        {
            Date date = new Date(2001, 365);
            Date expected = new Date(2001, 12, 31);
            Assert.Equal(expected, date);
        }

        [Fact]
        public void CanConstructDateFromYearAndDayOfYear_Leap()
        {
            Date date = new Date(2000, 366);
            Date expected = new Date(2000, 12, 31);
            Assert.Equal(expected, date);
        }

        [Fact]
        public void CannotConstructDateFromYearAndDayOfYear_NonLeap_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(2001, 366));
        }

        [Fact]
        public void CannotConstructDateFromYearAndDayOfYear_Leap_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(2000, 367));
        }

        [Fact]
        public void CannotConstructDateFromYearAndDayOfYear_TooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(2000, 0));
        }
    }
}
