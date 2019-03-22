using System;
using Kros.Caching;
using FluentAssertions;
using Xunit;

namespace Kros.Utils.UnitTests.Caching
{
    public class CacheShould
    {
        [Fact]
        public void GetValue()
        {
            var cache = new Cache<int, Foo>();
            var expected = "111";

            var actual = cache.Get(111, () => new Foo() { Value = "111" });

            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void ReturnCachedValue()
        {
            var cache = new Cache<string, Foo>();

            var actual = cache.Get("car", ()=> new Foo() { Value = "car" });

            actual = cache.Get("car", () => new Foo() { Value = "car_new"});

            actual.Value.Should().Be("car");
        }

        [Fact]
        public void UseFactoryForGetingValueAfterClear()
        {
            var cache = new Cache<string, Foo>();

            var actual = cache.Get("car", () => new Foo() { Value = "car" });

            cache.Clear();

            actual = cache.Get("car", () => new Foo() { Value = "car_new" });

            actual.Value.Should().Be("car_new");
        }

        [Fact]
        public void UseCustomEqualityComparer()
        {
            var cache = new Cache<string, int>(StringComparer.InvariantCultureIgnoreCase);

            cache.Get("test", () => 111);

            var actual = cache.Get("TeSt", () => 999);

            actual.Should().Be(111);
        }

        class Foo
        {
            public string Value { get; set; }
        }
    }
}
