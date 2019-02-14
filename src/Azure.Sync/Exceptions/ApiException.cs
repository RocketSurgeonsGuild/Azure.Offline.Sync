using System;
using System.Net.Http;

namespace Rocket.Surgery.Azure.Sync
{
    /// <inheritdoc />
    public class ApiSyncException : InvalidOperationException
    {
        public HttpRequestMessage RequestMessge { get; set; }

        public HttpResponseMessage ResponseMessage { get; set; }

        public ApiSyncException(
            string message,
            Exception innerException,
            HttpRequestMessage request,
            HttpResponseMessage response)
            : base(message, innerException)
        {
            RequestMessge = request;
            ResponseMessage = response;
        }
    }
}