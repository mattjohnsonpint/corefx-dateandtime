using Xunit;

namespace System.DateAndTime.Tests
{
    public class TimeOfDayFormattingTests
    {
        [Fact]
        public void ToLongDateString()
        {
            var time = new TimeOfDay(10, 49, 12, Meridiem.PM);
            var longTimeString = time.ToLongTimeString();
            Assert.Equal(longTimeString, "10:49:12 PM");
        }

        [Fact]
        public void ToShortDateString()
        {
            var time = new TimeOfDay(22, 49);
            var shortTimeString = time.ToShortTimeString();
            Assert.Equal(shortTimeString, "10:49 PM");
        }
    }
}
