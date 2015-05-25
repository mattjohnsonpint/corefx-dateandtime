using Xunit;

namespace System.DateAndTime.Tests
{
    public class DateMathTests
    {
        [Fact]
        public void CanAddPositiveYears()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddYears(1);
            var expected = new Date(2001, 1, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeYears()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddYears(-1);
            var expected = new Date(1999, 1, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddZeroYears()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddYears(0);
            var expected = new Date(2000, 1, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CannotAddYearsMoreThanMaxDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddYears(1);
            });
        }

        [Fact]
        public void CannotAddYearsLessThanMinDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddYears(-1);
            });
        }

        [Fact]
        public void CannotAddYearsMoreThanMaxPossibleYears()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddYears(10000);
            });
        }

        [Fact]
        public void CannotAddYearsLessThanMinPossibleYears()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddYears(-10000);
            });
        }

        [Fact]
        public void CanAddPositiveYears_FromLeapDay_ToNonLeapYear()
        {
            var dt = new Date(2000, 2, 29);
            var actual = dt.AddYears(1);
            var expected = new Date(2001, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeYears_FromLeapDay_ToNonLeapYear()
        {
            var dt = new Date(2000, 2, 29);
            var actual = dt.AddYears(-1);
            var expected = new Date(1999, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveYears_FromLeapDay_ToLeapYear()
        {
            var dt = new Date(2000, 2, 29);
            var actual = dt.AddYears(4);
            var expected = new Date(2004, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeYears_FromLeapDay_ToLeapYear()
        {
            var dt = new Date(2000, 2, 29);
            var actual = dt.AddYears(-4);
            var expected = new Date(1996, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveMonths()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddMonths(1);
            var expected = new Date(2000, 2, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddMonths(-1);
            var expected = new Date(1999, 12, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddZeroMonths()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddMonths(0);
            var expected = new Date(2000, 1, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CannotAddMonthsMoreThanMaxDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddMonths(1);
            });
        }

        [Fact]
        public void CannotAddMonthsLessThanMinDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddMonths(-1);
            });
        }

        [Fact]
        public void CannotAddMonthsMoreThanMaxPossibleMonths()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddMonths(120000);
            });
        }

        [Fact]
        public void CannotAddMonthsLessThanMinPossibleMonths()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddMonths(-120000);
            });
        }

        [Fact]
        public void CanAddPositiveMonths_FromDay31_ToDay30()
        {
            var dt = new Date(2000, 3, 31);
            var actual = dt.AddMonths(1);
            var expected = new Date(2000, 4, 30);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths_FromDay31_ToDay30()
        {
            var dt = new Date(2000, 5, 31);
            var actual = dt.AddMonths(-1);
            var expected = new Date(2000, 4, 30);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveMonths_FromDay31_ToDay29()
        {
            var dt = new Date(2000, 1, 31);
            var actual = dt.AddMonths(1);
            var expected = new Date(2000, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths_FromDay31_ToDay29()
        {
            var dt = new Date(2000, 3, 31);
            var actual = dt.AddMonths(-1);
            var expected = new Date(2000, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveMonths_FromDay31_ToDay28()
        {
            var dt = new Date(2001, 1, 31);
            var actual = dt.AddMonths(1);
            var expected = new Date(2001, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths_FromDay31_ToDay28()
        {
            var dt = new Date(2001, 3, 31);
            var actual = dt.AddMonths(-1);
            var expected = new Date(2001, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveMonths_FromDay30_ToDay29()
        {
            var dt = new Date(1999, 11, 30);
            var actual = dt.AddMonths(3);
            var expected = new Date(2000, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths_FromDay30_ToDay29()
        {
            var dt = new Date(2000, 4, 30);
            var actual = dt.AddMonths(-2);
            var expected = new Date(2000, 2, 29);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveMonths_FromDay30_ToDay28()
        {
            var dt = new Date(2000, 11, 30);
            var actual = dt.AddMonths(3);
            var expected = new Date(2001, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeMonths_FromDay30_ToDay28()
        {
            var dt = new Date(2001, 4, 30);
            var actual = dt.AddMonths(-2);
            var expected = new Date(2001, 2, 28);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddPositiveDays()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddDays(1);
            var expected = new Date(2000, 1, 2);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddZeroDays()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddDays(0);
            var expected = new Date(2000, 1, 1);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanAddNegativeDays()
        {
            var dt = new Date(2000, 1, 1);
            var actual = dt.AddDays(-1);
            var expected = new Date(1999, 12, 31);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CannotAddDaysMoreThanMaxDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddDays(1);
            });
        }

        [Fact]
        public void CannotAddDaysLessThanMinDate()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddDays(-1);
            });
        }

        [Fact]
        public void CannotAddDaysMoreThanMaxPossibleDays()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MinValue.AddDays(3652059);
            });
        }

        [Fact]
        public void CannotAddDaysLessThanMinPossibleDays()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = Date.MaxValue.AddDays(-3652059);
            });
        }
    }
}
