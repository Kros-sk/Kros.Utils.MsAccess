using FluentAssertions;
using Kros.Net;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Kros.Utils.UnitTests.Net
{
    public class HttpResponseMessageExtensionsTests
    {
        private const string HtmlContent = @"<html>
<head>
    <meta charset=""UTF-8"">
</head>
<body>
    <form method=""post"" action=""/"">
        <h3>Login</h3>
        <label for=""Email"">E-mail</label>
        <input type=""email"" id=""Email"" name=""Email"" value="""" />
        <label for=""Password"">Password</label>
        <input type=""password"" id=""Password"" name=""Password"" />
        <input name=""__RequestVerificationToken"" type=""hidden"" value=""anti-forgery-token"" />
        <input name=""RememberMe"" type=""hidden"" value=""false"" />
    </form>
  </body>
</html>";

        [Fact]
        public async void AntiForgeryTokenMustBeNullIfResponseIsNotSuccessful()
        {
            var message = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(HtmlContent)
            };
            string token = await message.GetAntiForgeryTokenAsync();
            token.Should().BeNull();
        }

        [Fact]
        public async void ShouldExtractAntiForgeryToken()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(HtmlContent)
            };
            string token = await message.GetAntiForgeryTokenAsync();
            token.Should().Be("anti-forgery-token");
        }

        [Fact]
        public void CookieShouldBeSetAndRetrieved()
        {
            var response = new HttpResponseMessage();
            response.SetCookie("cookie1", "value1");

            var expectedCookie = new SetCookieHeaderValue("cookie1", "value1");

            IList<SetCookieHeaderValue> actualCookies = response.GetCookies();
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

            var response = new HttpResponseMessage();
            response.SetCookies(cookies);

            var expectedCookies = new List<SetCookieHeaderValue>()
            {
                new SetCookieHeaderValue("cookie1", "value1"),
                new SetCookieHeaderValue("cookie2", "value2"),
                new SetCookieHeaderValue("cookie3", "value3")
            };

            IList<SetCookieHeaderValue> actualCookies = response.GetCookies();
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

            var response = new HttpResponseMessage();
            response.SetCookies(cookies);

            IDictionary<string, string> actualCookies = response.GetCookieValues();
            actualCookies.Should().BeEquivalentTo(cookies);
        }
    }
}
