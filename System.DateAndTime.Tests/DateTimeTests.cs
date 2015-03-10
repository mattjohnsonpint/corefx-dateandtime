using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateTimeTests
    {
        [Fact]
        public void CanAssignDateTimeTodayToDate()
        {
            Date dt = DateTime.Today;
        }

        [Fact]
        public void CanAssignDateTimeTimeOfDayToTimeOfDay()
        {
            TimeOfDay time = DateTime.Now.TimeOfDay;
        }
    }
}
