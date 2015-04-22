using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateFormattingTests
    {
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
