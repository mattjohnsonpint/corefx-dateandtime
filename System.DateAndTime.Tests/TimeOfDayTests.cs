using Xunit;

namespace System.DateAndTime.Tests
{
    public class TimeOfDayTests
    {
        [Fact]
        public void CanConstructDefaultTimeOfDay()
        {
            TimeOfDay time = new TimeOfDay();
            Assert.Equal(0, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFromTicks()
        {
            TimeOfDay time = new TimeOfDay(863999999999L);
            Assert.Equal(863999999999L, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom24HoursAndMinutes()
        {
            TimeOfDay time = new TimeOfDay(23, 59);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute;
            Assert.Equal(expected, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom12HoursAndMinutes()
        {
            TimeOfDay time = new TimeOfDay(11, 59, Meridiem.PM);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute;
            Assert.Equal(expected, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom24HoursAndMinutesAndSeconds()
        {
            TimeOfDay time = new TimeOfDay(23, 59, 59);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute +
                                  59 * TimeSpan.TicksPerSecond;
            Assert.Equal(expected, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom12HoursAndMinutesAndSeconds()
        {
            TimeOfDay time = new TimeOfDay(11, 59, 59, Meridiem.PM);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute +
                                  59 * TimeSpan.TicksPerSecond;
            Assert.Equal(expected, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom24HoursAndMinutesAndSecondsAndMilliseconds()
        {
            TimeOfDay time = new TimeOfDay(23, 59, 59, 59);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute +
                                  59 * TimeSpan.TicksPerSecond +
                                  59 * TimeSpan.TicksPerMillisecond;
            Assert.Equal(expected, time.Ticks);
        }

        [Fact]
        public void CanConstructTimeOfDayFrom12HoursAndMinutesAndSecondsAndMilliseconds()
        {
            TimeOfDay time = new TimeOfDay(11, 59, 59, 59, Meridiem.PM);
            const long expected = 23 * TimeSpan.TicksPerHour +
                                  59 * TimeSpan.TicksPerMinute +
                                  59 * TimeSpan.TicksPerSecond +
                                  59 * TimeSpan.TicksPerMillisecond;
            Assert.Equal(expected, time.Ticks);
        }


        [Fact]
        public void CanGetHour24FromTimeOfDay()
        {
            TimeOfDay time = new TimeOfDay(23, 0);
            Assert.Equal(23, time.Hours24);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_12AM()
        {
            TimeOfDay time = new TimeOfDay(0, 0);
            Assert.Equal(12, time.Hours12);
            Assert.Equal(Meridiem.AM, time.Meridiem);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_01AM()
        {
            TimeOfDay time = new TimeOfDay(1, 0);
            Assert.Equal(1, time.Hours12);
            Assert.Equal(Meridiem.AM, time.Meridiem);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_11AM()
        {
            TimeOfDay time = new TimeOfDay(11, 0);
            Assert.Equal(11, time.Hours12);
            Assert.Equal(Meridiem.AM, time.Meridiem);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_12PM()
        {
            TimeOfDay time = new TimeOfDay(12, 0);
            Assert.Equal(12, time.Hours12);
            Assert.Equal(Meridiem.PM, time.Meridiem);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_01PM()
        {
            TimeOfDay time = new TimeOfDay(13, 0);
            Assert.Equal(1, time.Hours12);
            Assert.Equal(Meridiem.PM, time.Meridiem);
        }

        [Fact]
        public void CanGetHour12FromTimeOfDay_11PM()
        {
            TimeOfDay time = new TimeOfDay(23, 0);
            Assert.Equal(11, time.Hours12);
            Assert.Equal(Meridiem.PM, time.Meridiem);
        }

        [Fact]
        public void CanGetDateTimeFromTimeOnDate()
        {
            Date date = new Date(2000, 12, 31);
            TimeOfDay time = new TimeOfDay(23, 59, 59);
            DateTime dt = time.On(date);

            DateTime expected = new DateTime(2000, 12, 31, 23, 59, 59);
            Assert.Equal(expected, dt);
        }

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

        [Fact]
        public void CanCalculateDuration_Normal()
        {
            TimeOfDay startTime = new TimeOfDay(10, 0);
            TimeOfDay endTime = new TimeOfDay(12, 0);

            TimeSpan duration = TimeOfDay.CalculateDuration(startTime, endTime);
            Assert.Equal(TimeSpan.FromHours(2), duration);
        }

        [Fact]
        public void CanCalculateDuration_OverMidnight()
        {
            TimeOfDay startTime = new TimeOfDay(23, 0);
            TimeOfDay endTime = new TimeOfDay(1, 0);

            TimeSpan duration = TimeOfDay.CalculateDuration(startTime, endTime);
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
