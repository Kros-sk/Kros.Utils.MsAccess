using FluentAssertions;
using Kros.Net;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Kros.Utils.UnitTests.Net
{
    public class HttpRequestMessageExtensionsTests
    {
        [Fact]
        public void CookieShouldBeSetAndRetrieved()
        {
            var request = new HttpRequestMessage();
            request.SetCookie("cookie1", "value1");

            var expectedCookie = new CookieHeaderValue("cookie1", "value1");

            IList<CookieHeaderValue> actualCookies = request.GetCookies();
            actualCookies.Should().BeEquivalentTo(new[] { expectedCookie });
        }

        [Fact]
        public void CookiesShouldBeSetAndRetrieved()
        {
            var cookies = new Dictionary<string, string>()
            {
                { "cookie1", "value1" },
                { "cookie2", "value2" },
                { "cookie3", "value3" }
            };

            var request = new HttpRequestMessage();
            request.SetCookies(cookies);

            var expectedCookies = new List<CookieHeaderValue>()
            {
                new CookieHeaderValue("cookie1", "value1"),
                new CookieHeaderValue("cookie2", "value2"),
                new CookieHeaderValue("cookie3", "value3")
            };

            IList<CookieHeaderValue> actualCookies = request.GetCookies();
            actualCookies.Should().BeEquivalentTo(expectedCookies);
        }

        [Fact]
        public void CookiesShouldBeSetAndRetrievedAsDictionary()
        {
            var cookies = new Dictionary<string, string>()
            {
                { "cookie1", "value1" },
                { "cookie2", "value2" },
                { "cookie3", "value3" }
            };

            var request = new HttpRequestMessage();
            request.SetCookies(cookies);

            IDictionary<string, string> actualCookies = request.GetCookieValues();
            actualCookies.Should().BeEquivalentTo(cookies);
        }

        [Fact]
        public void ShouldCopyCookiesToRequestFromResponse()
        {
            var cookies = new Dictionary<string, string>()
            {
                { "cookie1", "value1" },
                { "cookie2", "value2" },
                { "cookie3", "value3" },
                { "cookie4", "value4" }
            };

            var response = new HttpResponseMessage();
            response.Headers.Add(HeaderNames.SetCookie, "cookie1=value1");
            response.Headers.Add(HeaderNames.SetCookie, "cookie2=value2");
            response.Headers.Add(HeaderNames.SetCookie, "cookie3=value3; Max-Age=123");
            response.Headers.Add(HeaderNames.SetCookie, "cookie4=value4; Domain=example.com; Secure;");

            var request = new HttpRequestMessage();
            request.CopyCookiesFromResponse(response);

            IDictionary<string, string> requestCookies = request.GetCookieValues();
            requestCookies.Should().BeEquivalentTo(cookies);
        }
    }
}
