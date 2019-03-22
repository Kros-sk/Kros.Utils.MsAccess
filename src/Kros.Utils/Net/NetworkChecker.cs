using Kros.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace Kros.Net
{
    /// <summary>
    /// Class dedicated for simple testing of internet connectivity.
    /// </summary>
    /// <remarks>
    /// It is not sufficient to test connectivity using <see cref="NetworkInterface.GetIsNetworkAvailable()"/>, because that
    /// method just checks, if the computer is in some network. It does not check if internet is really available.
    /// Internet availability is not checked using ping (<see cref="Ping"/>), because this method is often blocked.
    /// The availability is tested using a request to specific service.
    /// </remarks>
    public class NetworkChecker
    {
        #region Fields

        private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultResponseCacheExpiration = TimeSpan.FromMinutes(3);
        private DateTime _lastSuccessResponseTime;
        private readonly Func<HttpMessageHandler> _httpMessageHandlerFactory;

        #endregion

        #region Constructors

        /// <inheritdoc cref="NetworkChecker(Uri, Uri, TimeSpan, TimeSpan)"/>
        public NetworkChecker(Uri serviceAddress)
            : this(serviceAddress, (Uri) null, DefaultRequestTimeout, DefaultResponseCacheExpiration)
        {
        }

        /// <inheritdoc cref="NetworkChecker(Uri, Uri, TimeSpan, TimeSpan)"/>
        public NetworkChecker(Uri serviceAddress, Uri proxyAddress)
            : this(serviceAddress, proxyAddress, DefaultRequestTimeout, DefaultResponseCacheExpiration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChecker"/> class.
        /// </summary>
        /// <param name="serviceAddress">The address for requests checking internet availability. It must be <c>http</c>
        /// or <c>https</c> address.</param>
        /// <param name="httpMessageHandlerFactory">Factory function to create <see cref="HttpMessageHandler"/>
        /// which will be used.</param>
        public NetworkChecker(Uri serviceAddress, Func<HttpMessageHandler> httpMessageHandlerFactory)
            : this(serviceAddress, httpMessageHandlerFactory, DefaultRequestTimeout, DefaultResponseCacheExpiration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChecker"/> class.
        /// </summary>
        /// <param name="serviceAddress">The address for requests checking internet availability. It must be <c>http</c>
        /// or <c>https</c> address.</param>
        /// <param name="httpMessageHandlerFactory">Factory function to create <see cref="HttpMessageHandler"/>
        /// which will be used.</param>
        /// <param name="requestTimeout">Maximum time for waiting for the response from server. If the response will not
        /// came in this time, we consider that the internet is not available.</param>
        /// <param name="responseCacheExpiration">Time during which the last response will be remembered
        /// and so no requests to <paramref name="serviceAddress"/> will be performed.
        /// </param>
        public NetworkChecker(
            Uri serviceAddress,
            Func<HttpMessageHandler> httpMessageHandlerFactory,
            TimeSpan requestTimeout,
            TimeSpan responseCacheExpiration)
            : this(serviceAddress, (Uri) null, requestTimeout, responseCacheExpiration)
        {
            _httpMessageHandlerFactory = Check.NotNull(httpMessageHandlerFactory, nameof(httpMessageHandlerFactory));
        }

        /// <inheritdoc cref="NetworkChecker(Uri, Uri, TimeSpan, TimeSpan)"/>
        public NetworkChecker(Uri serviceAddress, TimeSpan requestTimeout, TimeSpan responseCacheExpiration)
            : this(serviceAddress, (Uri) null, requestTimeout, responseCacheExpiration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChecker"/> with address <paramref name="serviceAddress"/>
        /// and aditional parameters.
        /// </summary>
        /// <param name="serviceAddress">The address for requests checking internet availability. It must be <c>http</c>
        /// or <c>https</c> address.</param>
        /// <param name="proxyAddress">The address of a proxy server (optional).</param>
        /// <param name="requestTimeout">Maximum time for waiting for the response from server. If the response will not
        /// came in this time, we consider that the internet is not available.</param>
        /// <param name="responseCacheExpiration">Time during which the last response will be remembered
        /// and so no requests to <paramref name="serviceAddress"/> will be performed.
        /// </param>
        public NetworkChecker(
            Uri serviceAddress,
            Uri proxyAddress,
            TimeSpan requestTimeout,
            TimeSpan responseCacheExpiration)
        {
            ServiceAddress = Check.NotNull(serviceAddress, nameof(serviceAddress));
            ProxyAddress = proxyAddress;
            RequestTimeout = Check.GreaterOrEqualThan(requestTimeout, TimeSpan.Zero, nameof(requestTimeout));
            ResponseCacheExpiration = Check.GreaterOrEqualThan(responseCacheExpiration, TimeSpan.Zero,
                nameof(responseCacheExpiration));
        }

        #endregion

        #region NetworkChecker

        /// <summary>
        /// Web address to which requests are made to check internet availability.
        /// </summary>
        public Uri ServiceAddress { get; }

        /// <summary>
        /// Address of a proxy server.
        /// </summary>
        public Uri ProxyAddress { get; }

        /// <summary>
        /// Maximum time for waiting for the response from server. If the response will not
        /// came in this time, we consider that the internet is not available. Default timeout is 1 second.
        /// </summary>
        public TimeSpan RequestTimeout { get; }

        /// <summary>
        /// Time during which the last response will be remembered and so no other requests to <see cref="ServiceAddress"/>
        /// will be performed. Default value is 3 minutes.
        /// </summary>
        public TimeSpan ResponseCacheExpiration { get; }

        /// <summary>
        /// Checks if the internet (specifically the service at the address <see cref="ServiceAddress"/>) is available.
        /// Positive response is cached for the time specified in <see cref="ResponseCacheExpiration"/>,
        /// so another request to the service is made after this time.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if internet (service) is available <see langword="false"/> otherwise.
        /// </returns>
        public bool IsNetworkAvailable() => CheckNetwork() && (HasCachedResponse() || CheckService());

        internal virtual bool CheckNetwork() => NetworkInterface.GetIsNetworkAvailable();

        private bool HasCachedResponse() =>
            _lastSuccessResponseTime.Add(ResponseCacheExpiration) >= DateTimeProvider.Now;

        private bool CheckService()
        {
            try
            {
                using (var client = new HttpClient(CreateMessageHandler()) { Timeout = RequestTimeout })
                using (var stream = client.GetStreamAsync(ServiceAddress).GetAwaiter().GetResult())
                {
                    _lastSuccessResponseTime = DateTimeProvider.Now;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        internal virtual HttpMessageHandler CreateMessageHandler()
        {
            if (_httpMessageHandlerFactory != null)
            {
                return _httpMessageHandlerFactory();
            }
            else
            {
                var handler = new HttpClientHandler();
                if (ProxyAddress != null)
                {
                    handler.Proxy = new WebProxy(ProxyAddress);
                }
                return handler;
            }
        }

        #endregion
    }
}
