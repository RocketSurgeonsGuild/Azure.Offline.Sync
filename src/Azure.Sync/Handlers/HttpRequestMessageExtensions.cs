using System.Net.Http;

namespace Rocket.Surgery.Azure.Sync.Handlers
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Add the desired api version to the Accept header of the <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        public static HttpRequestMessage WithVersion(this HttpRequestMessage requestMessage, int apiVersion)
        {
            requestMessage.Headers.Accept.Clear();
            requestMessage.Headers.Add("Accept", $"application/json;ver={apiVersion}");
            return requestMessage;
        }
    }
}