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
        public void CanConstructDateFromParts()
        {
            Date date = new Date(9999, 12, 31);
            Assert.Equal(3652058, date.DayNumber);
        }
    }
}
