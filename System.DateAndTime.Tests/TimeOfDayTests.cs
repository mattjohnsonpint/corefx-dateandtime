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
    }
}
