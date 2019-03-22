#if netcoreapp
using Kros.Utils;
using Microsoft.Net.Http.Headers;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Kros.Net
{
    /// <summary>
    /// Extensions method for <see cref="HttpRequestMessage"/> class.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
#if netcoreapp
        /// <summary>
        /// Sets cookie <paramref name="cookie"/> to the request.
        /// </summary>
        /// <param name="request">HTTP request to which the cookie is set.</param>
        /// <param name="cookie">The cookie to be set.</param>
        /// <returns>Returns back original HTTP request for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpRequestMessage SetCookie(this HttpRequestMessage request, CookieHeaderValue cookie)
        {
            request.Headers.Add(HeaderNames.Cookie, cookie.ToString());
            return request;
        }

        /// <summary>
        /// Sets cookie with name <paramref name="cookieName"/> and value <paramref name="cookieValue"/> to the request.
        /// </summary>
        /// <param name="request">HTTP request to which the cookie is set.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="cookieValue">Value of the cookie.</param>
        /// <returns>Returns back original HTTP request for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpRequestMessage SetCookie(this HttpRequestMessage request, string cookieName, string cookieValue)
        {
            Check.NotNullOrEmpty(cookieName, nameof(cookieName));
            request.Headers.Add(HeaderNames.Cookie, new CookieHeaderValue(cookieName, cookieValue).ToString());
            return request;
        }

        /// <summary>
        /// Sets cookies <paramref name="cookies"/> to the request <paramref name="request"/>.
        /// </summary>
        /// <param name="request">HTTP request to which the cookies are set.</param>
        /// <param name="cookies">Cookies set to the request.</param>
        /// <returns>Returns back original HTTP request for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpRequestMessage SetCookies(this HttpRequestMessage request, IDictionary<string, string> cookies)
        {
            foreach (KeyValuePair<string, string> cookie in cookies)
            {
                request.Headers.Add(HeaderNames.Cookie, new CookieHeaderValue(cookie.Key, cookie.Value).ToString());
            }
            return request;
        }

        /// <summary>
        /// Sets cookies <paramref name="cookies"/> to the request.
        /// </summary>
        /// <param name="request">HTTP request to which the cookies are set.</param>
        /// <param name="cookies">Cookies set to the request.</param>
        /// <returns>Returns back original HTTP request for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpRequestMessage SetCookies(this HttpRequestMessage request, IEnumerable<CookieHeaderValue> cookies)
        {
            foreach (CookieHeaderValue cookie in cookies)
            {
                request.Headers.Add(HeaderNames.Cookie, cookie.ToString());
            }
            return request;
        }

        /// <summary>
        /// Gets cookies from the HTTP request.
        /// </summary>
        /// <param name="request">HTTP request from which the cookies are retrieved.</param>
        /// <returns>
        /// List of <see cref="CookieHeaderValue"/> objects. If the request does not have any cookies, empty list is returned.
        /// </returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static IList<CookieHeaderValue> GetCookies(this HttpRequestMessage request)
        {
            if (request.Headers.TryGetValues(HeaderNames.Cookie, out IEnumerable<string> values))
            {
                return CookieHeaderValue.ParseList(values.ToList());
            }
            return new List<CookieHeaderValue>();
        }

        /// <summary>
        /// Gets cookie values from the HTTP request.
        /// </summary>
        /// <param name="request">HTTP request from which the cookies are retrieved.</param>
        /// <returns>The dictionary of the cookies and their values. If the request does not have any cookies,
        /// empty dictionary is returned.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static IDictionary<string, string> GetCookieValues(this HttpRequestMessage request)
        {
            var result = new Dictionary<string, string>();
            foreach (CookieHeaderValue cookie in request.GetCookies())
            {
                result.Add(cookie.Name.Value, cookie.Value.Value);
            }
            return result;
        }

        /// <summary>
        /// Copies all the cookies from the HTTP response <paramref name="response"/> to the HTTP request.
        /// </summary>
        /// <param name="request">HTTP request to which the cookies are copied.</param>
        /// <param name="response">HTTP request from which the cookies are copied.</param>
        /// <returns>Returns back original HTTP request for fluent API.</returns>
        /// <remarks><para>This method is available only in .NET Core version of the library.</para></remarks>
        public static HttpRequestMessage CopyCookiesFromResponse(this HttpRequestMessage request, HttpResponseMessage response)
            => request.SetCookies(response.GetCookieValues());
#endif
    }
}
