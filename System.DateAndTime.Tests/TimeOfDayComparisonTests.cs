using Xunit;

namespace System.DateAndTime.Tests
{
    public class TimeOfDayComparisonTests
    {
        [Fact]
        public void CanDetermineTimeInBetween_NormalInside()
        {
            TimeOfDay startTime = new TimeOfDay(10, 0);
            TimeOfDay testTime = new TimeOfDay(10, 0);
            TimeOfDay endTime = new TimeOfDay(12, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.True(between);
        }

        [Fact]
        public void CanDetermineTimeInBetween_NormalBefore()
        {
            TimeOfDay testTime = new TimeOfDay(9, 0);
            TimeOfDay startTime = new TimeOfDay(10, 0);
            TimeOfDay endTime = new TimeOfDay(12, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.False(between);
        }

        [Fact]
        public void CanDetermineTimeInBetween_NormalAfter()
        {
            TimeOfDay startTime = new TimeOfDay(10, 0);
            TimeOfDay endTime = new TimeOfDay(12, 0);
            TimeOfDay testTime = new TimeOfDay(12, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.False(between);
        }

        [Fact]
        public void CanDetermineTimeInBetween_OverMidnightInside()
        {
            TimeOfDay startTime = new TimeOfDay(23, 0);
            TimeOfDay testTime = new TimeOfDay(23, 0);
            TimeOfDay endTime = new TimeOfDay(1, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.True(between);
        }

        [Fact]
        public void CanDetermineTimeInBetween_OverMidnightBefore()
        {
            TimeOfDay testTime = new TimeOfDay(22, 0);
            TimeOfDay startTime = new TimeOfDay(23, 0);
            TimeOfDay endTime = new TimeOfDay(1, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.False(between);
        }

        [Fact]
        public void CanDetermineTimeInBetween_OverMidnightAfter()
        {
            TimeOfDay startTime = new TimeOfDay(23, 0);
            TimeOfDay endTime = new TimeOfDay(1, 0);
            TimeOfDay testTime = new TimeOfDay(1, 0);

            bool between = testTime.IsBetween(startTime, endTime);
            Assert.False(between);
        }
    }
}
