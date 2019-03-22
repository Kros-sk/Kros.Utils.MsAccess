using FluentAssertions;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Utils
{
    public class DateTimeProviderShould
    {
        [Fact]
        public void ReturnCorrectDateTime()
        {
            DateTimeProvider.Now.Should().BeSameDateAs(DateTime.Now);
        }

        [Fact]
        public void InjectCustomDateTime()
        {
            var expected = new DateTime(2017, 10, 11, 5, 22, 33, 11);
            using (DateTimeProvider.InjectActualDateTime(expected))
            {
                DateTimeProvider.Now.Should().Be(expected);
            }
        }

        [Fact]
        public void ReturnCorrectDateAfterDispose()
        {
            var expected = new DateTime(2017, 10, 11, 5, 22, 33, 11);
            using (DateTimeProvider.InjectActualDateTime(expected))
            {
            }

            DateTimeProvider.Now.Should().BeSameDateAs(DateTime.Now);
        }
    }
}
