using System.Globalization;
using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateCalendarTests
    {
        [Fact]
        public void CanCreateDateWithCalendar()
        {
            var actual = new Date(1436, 3, 10, new UmAlQuraCalendar());
            var expected = new Date(2015, 1, 1);
            Assert.Equal(expected, actual);
        }
    }
}
