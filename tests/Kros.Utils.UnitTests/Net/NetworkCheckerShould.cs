using FluentAssertions;
using Kros.Net;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Net
{
    public class NetworkCheckerShould
    {
        #region Nested classes

        private class FakeMessageHandler : HttpMessageHandler
        {
            private readonly bool _throwException;

            public FakeMessageHandler(bool throwExpcetion)
            {
                _throwException = throwExpcetion;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                if (_throwException)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return Task.FromResult(new HttpResponseMessage());
                }
            }
        }

        private class TestNetworkChecker : NetworkChecker
        {
            public TestNetworkChecker(bool throwException)
                : base(new Uri("https://www.kros.sk"), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(200))
            {
                ThrowException = throwException;
            }

            public bool CheckNetworkResponse { get; set; } = true;

            public bool ThrowException { get; set; }

            internal override HttpMessageHandler CreateMessageHandler()
            {
                return new FakeMessageHandler(ThrowException);
            }

            internal override bool CheckNetwork() => CheckNetworkResponse;
        }

        #endregion

        #region Tests

        [Fact]
        public void ReturnTrueIfInternetIsAvailable()
        {
            var checker = new TestNetworkChecker(throwException: false);

            checker.IsNetworkAvailable().Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseIfInternetIsNotAvailable()
        {
            var checker = new TestNetworkChecker(throwException: true);

            checker.IsNetworkAvailable().Should().BeFalse();
        }

        [Fact]
        public void CacheLastSuccess()
        {
            var checker = new TestNetworkChecker(throwException: false);

            using (DateTimeProvider.InjectActualDateTime(new DateTime(2017, 10, 11, 15, 3, 3, 10)))
            {
                var available = checker.IsNetworkAvailable();
            }

            using (DateTimeProvider.InjectActualDateTime(new DateTime(2017, 10, 11, 15, 3, 3, 209)))
            {
                checker.ThrowException = true;
                checker.IsNetworkAvailable().Should().BeTrue();
            }
        }

        [Fact]
        public void NotUseCacheIfExpired()
        {
            var checker = new TestNetworkChecker(throwException: false);

            using (DateTimeProvider.InjectActualDateTime(new DateTime(2017, 10, 11, 15, 3, 3, 10)))
            {
                var available = checker.IsNetworkAvailable();
            }

            using (DateTimeProvider.InjectActualDateTime(new DateTime(2017, 10, 11, 15, 3, 3, 211)))
            {
                checker.ThrowException = true;
                checker.IsNetworkAvailable().Should().BeFalse();
            }
        }

        [Fact]
        public void CheckLocalNetwork()
        {
            var checker = new TestNetworkChecker(throwException: false);
            checker.CheckNetworkResponse = false;

            checker.IsNetworkAvailable().Should().BeFalse();
        }

        #endregion
    }
}
