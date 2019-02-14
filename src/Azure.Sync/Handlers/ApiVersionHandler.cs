using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.Azure.Sync.Handlers
{
    /// <inheritdoc />
    public class ApiVersionHandler : DelegatingHandler
    {
        private readonly int _apiVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionHandler"/> class.
        /// </summary>
        /// <param name="apiVersion">The API version.</param>
        /// <inheritdoc />
        public ApiVersionHandler(int apiVersion) { _apiVersion = apiVersion; }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message,
            CancellationToken token)
        {
            // Do any pre-request requirements here
            message.WithVersion(_apiVersion);

            // Request happens here
            var response = await base.SendAsync(message, token);

            // Do any post-request requirements here

            return await Task.FromResult(response);
        }
    }
}
