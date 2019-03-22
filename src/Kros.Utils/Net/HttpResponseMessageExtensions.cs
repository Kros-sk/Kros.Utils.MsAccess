#if netcoreapp
using Kros.Utils;
using Microsoft.Net.Http.Headers;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kros.Net
{
    /// <summary>
    /// Extensions method for <see cref="HttpResponseMessage"/> class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Extracts anti-forgery token from the HTTP response.
        /// Z HTTP odpovede získa hodnotu anti-forgery tokenu. Ten sa získa iba v prípade, že odpoveď je úspešná, tzn. jej
        /// <c>StatusCode</c> má hodnotu 2xx.
        /// </summary>
        /// <param name="response">HTTP response from which the anti-forgery token is extracted.</param>
        /// <returns>Value of the anti-forgery token or <see langword="null"/>, if the token was not found or the response
        /// is not successful.</returns>
        /// <remarks>
        /// The token is extracted only if the respons was successful (<see cref="HttpResponseMessage.IsSuccessStatusCode"/>).
        /// The response's content is searched for form's field with the name
        /// <see cref="HttpClientExtensions.AntiForgeryTokenFieldName"/>. If that field is found, it's value is returned.
        /// </remarks>
        public static async Task<string> GetAntiForgeryTokenAsync(this HttpResponseMessage response)
        {
            string token = null;
            if (response.IsSuccessStatusCode && (response.Content != null))
            {
                string content = await response.Content.ReadAsStringAsync();
                Match match = Regex.Match(
                    content,
                    $@"\<input name=""{HttpClientExtensions.AntiForgeryTokenFieldName}"" " +
                    @"type=""hidden"" value=""(?<token>[^""]+)"" \/\>");
                if (match.Success)
                {
                    token = match.Groups["token"].Captures[0].Value;
                }
            }
            return token;
        }

#if netcoreapp
        /// <summary>
        /// Sets cookie <paramref name="cookie"/> to the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response to which the cookie is set.</param>
        /// <param name="cookie">The cookie to be set.</param>
        /// <returns>Returns back original HTTP response for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpResponseMessage SetCookie(this HttpResponseMessage response, SetCookieHeaderValue cookie)
        {
            response.Headers.Add(HeaderNames.SetCookie, cookie.ToString());
            return response;
        }

        /// <summary>
        /// Sets cookie with name <paramref name="cookieName"/> and value <paramref name="cookieValue"/> to the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response to which the cookie is set.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="cookieValue">Value of the cookie.</param>
        /// <returns>Returns back original HTTP response for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpResponseMessage SetCookie(this HttpResponseMessage response, string cookieName, string cookieValue)
        {
            Check.NotNullOrEmpty(cookieName, nameof(cookieName));
            response.Headers.Add(HeaderNames.SetCookie, new SetCookieHeaderValue(cookieName, cookieValue).ToString());
            return response;
        }

        /// <summary>
        /// Sets cookies <paramref name="cookies"/> to the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response to which the cookies are set.</param>
        /// <param name="cookies">Cookies set to the response.</param>
        /// <returns>Returns back original HTTP response for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpResponseMessage SetCookies(this HttpResponseMessage response, IEnumerable<SetCookieHeaderValue> cookies)
        {
            foreach (SetCookieHeaderValue cookie in cookies)
            {
                response.Headers.Add(HeaderNames.SetCookie, cookie.ToString());
            }
            return response;
        }

        /// <summary>
        /// Sets cookies <paramref name="cookies"/> to the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response to which the cookies are set.</param>
        /// <param name="cookies">Cookies set to the response.</param>
        /// <returns>Returns back original HTTP response for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpResponseMessage SetCookies(this HttpResponseMessage response, IDictionary<string, string> cookies)
        {
            foreach (KeyValuePair<string, string> cookie in cookies)
            {
                response.Headers.Add(HeaderNames.SetCookie, new SetCookieHeaderValue(cookie.Key, cookie.Value).ToString());
            }
            return response;
        }

        /// <summary>
        /// Gets cookies from the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response from which the cookies are retrieved.</param>
        /// <returns>
        /// List of <see cref="SetCookieHeaderValue"/> objects. If the response does not have any cookies, empty list is returned.
        /// </returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static IList<SetCookieHeaderValue> GetCookies(this HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues(HeaderNames.SetCookie, out IEnumerable<string> values))
            {
                return SetCookieHeaderValue.ParseList(values.ToList());
            }
            return new List<SetCookieHeaderValue>();
        }

        /// <summary>
        /// Gets cookie values from the HTTP response.
        /// </summary>
        /// <param name="response">HTTP response from which the cookies are retrieved.</param>
        /// <returns>The dictionary of the cookies and their values. If the response does not have any cookies,
        /// empty dictionary is returned.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static IDictionary<string, string> GetCookieValues(this HttpResponseMessage response)
        {
            var result = new Dictionary<string, string>();
            foreach (SetCookieHeaderValue cookie in response.GetCookies())
            {
                result.Add(cookie.Name.Value, cookie.Value.Value);
            }
            return result;
        }
#endif
    }
}
