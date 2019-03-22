using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class IEnumerableExtensionsShould
    {
        [Fact]
        public void ShouldPerformSpecifiedAction()
        {
            var expected = new List<int>();
            IEnumerable<int> items = new List<int>() { 1, 2, 3, 4, 5 };

            void action(int value)
            {
                expected.Add(value);
            }

            items.ForEach(action);
            items.Should().Equal(expected);
        }
    }
}
