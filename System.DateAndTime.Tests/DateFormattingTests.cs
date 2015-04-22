using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateFormattingTests
    {
        [Fact]
        public void ToLongDateString()
        {
            var date = new Date(2000, 12, 31);
            var s = date.ToLongDateString();
            Assert.Equal("Sunday, December 31, 2000", s);
        }

        [Fact]
        public void ToShortDateString()
        {
            var date = new Date(2000, 12, 31);
            var s = date.ToShortDateString();
            Assert.Equal("12/31/2000", s);
        }

        [Fact]
        public void ToIsoString()
        {
            var date = new Date(2000, 12, 31);
            var s = date.ToIsoString();
            Assert.Equal("2000-12-31", s);
        }
    }
}
