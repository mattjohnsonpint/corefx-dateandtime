using Xunit;

namespace System.DateAndTime.Tests
{
    public class TimeOfDayFormattingTests
    {
        [Fact]
        public void ToLongDateString()
        {
            var time = new TimeOfDay(10, 49, 12, Meridiem.PM);
            var s = time.ToLongTimeString();
            Assert.Equal("10:49:12 PM", s);
        }

        [Fact]
        public void ToShortDateString()
        {
            var time = new TimeOfDay(22, 49);
            var s = time.ToShortTimeString();
            Assert.Equal("10:49 PM", s);
        }
    }
}
