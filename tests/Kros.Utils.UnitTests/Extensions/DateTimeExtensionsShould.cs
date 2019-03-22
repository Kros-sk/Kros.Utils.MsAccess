using FluentAssertions;
using Kros.Extensions;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class DateTimeExtensionsShould
    {
        [Fact]
        public void ReturnFirstDayOfMonth()
        {
            DateTime input = new DateTime(1978, 12, 10, 7, 30, 0);
            DateTime actual = input.FirstDayOfMonth();
            DateTime expected = new DateTime(1978, 12, 1, 0, 0, 0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ReturnFirstDayOfCurrentMonth()
        {
            DateTime actual = DateTimeExtensions.FirstDayOfCurrentMonth();
            DateTime expected = DateTime.Now.FirstDayOfMonth();
            actual.Should().Be(expected);
        }

        [Fact]

        public void ReturnLastDayOfMonth()
        {
            DateTime input = new DateTime(1978, 12, 10, 7, 30, 0);
            DateTime actual = input.LastDayOfMonth();
            DateTime expected = new DateTime(1978, 12, 31, 0, 0, 0);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ReturnLastDayOfCurrentMonth()
        {
            DateTime actual = DateTimeExtensions.LastDayOfCurrentMonth();
            DateTime expected = DateTime.Now.LastDayOfMonth();
            actual.Should().Be(expected);
        }

        [Fact]

        public void ReturnLastDayOfMonthInLeapYear()
        {
            DateTime input = new DateTime(2016, 2, 10, 7, 30, 0);
            DateTime actual = input.LastDayOfMonth();
            DateTime expected = new DateTime(2016, 2, 29, 0, 0, 0);
            actual.Should().Be(expected);
        }
    }
}
