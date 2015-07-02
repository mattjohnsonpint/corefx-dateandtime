using Xunit;

namespace System.DateAndTime.Tests
{
    public class TimeOfDayMathTests
    {
        [Fact]
        public void CanCalculateDuration_Normal()
        {
            TimeOfDay startTime = new TimeOfDay(10, 0);
            TimeOfDay endTime = new TimeOfDay(12, 0);

            TimeSpan duration = endTime - startTime;
            Assert.Equal(TimeSpan.FromHours(2), duration);
        }

        [Fact]
        public void CanCalculateDuration_OverMidnight()
        {
            TimeOfDay startTime = new TimeOfDay(23, 0);
            TimeOfDay endTime = new TimeOfDay(1, 0);

            TimeSpan duration = endTime - startTime;
            Assert.Equal(TimeSpan.FromHours(2), duration);
        }

        [Fact]
        public void CanAddPositiveTime()
        {
            TimeOfDay startTime = new TimeOfDay(12, 0);
            TimeOfDay actual = startTime.AddHours(13);
            TimeOfDay expected = new TimeOfDay(1, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeTime()
        {
            TimeOfDay startTime = new TimeOfDay(12, 0);
            TimeOfDay actual = startTime.AddHours(-13);
            TimeOfDay expected = new TimeOfDay(23, 0);

            Assert.Equal(expected, actual);
        }
    }
}
