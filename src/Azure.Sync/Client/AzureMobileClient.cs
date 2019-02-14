using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Eventing;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Azure.Sync.Abstractions.Client;

namespace Rocket.Surgery.Azure.Sync
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="IAzureMobileClient" />
    public class AzureMobileServiceClient : IAzureMobileClient
    {
        private readonly IMobileServiceClient _mobileServiceClient;

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the alternate login host.
        /// </summary>
        public Uri AlternateLoginHost { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public MobileServiceUser CurrentUser { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the login URI prefix.
        /// </summary>
        public string LoginUriPrefix { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the serializer settings.
        /// </summary>
        public MobileServiceJsonSerializerSettings SerializerSettings { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the event manager.
        /// </summary>
        public IMobileServiceEventManager EventManager => _mobileServiceClient.EventManager;

        /// <inheritdoc />
        /// <summary>
        /// Gets the installation identifier.
        /// </summary>
        public string InstallationId => _mobileServiceClient.InstallationId;

        /// <inheritdoc />
        /// <summary>
        /// Gets the mobile application URI.
        /// </summary>
        public Uri MobileAppUri => _mobileServiceClient.MobileAppUri;

        /// <inheritdoc />
        /// <summary>
        /// Gets the synchronize context.
        /// </summary>
        public IMobileServiceSyncContext SyncContext => _mobileServiceClient.SyncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureMobileServiceClient"/> class.
        /// </summary>
        /// <param name="mobileServiceClient">The mobile service client.</param>
        public AzureMobileServiceClient(IMobileServiceClient mobileServiceClient)
        {
            _mobileServiceClient = mobileServiceClient;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the synchronize table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public IMobileServiceSyncTable GetSyncTable(string tableName) => _mobileServiceClient.GetSyncTable(tableName);

        /// <inheritdoc />
        /// <summary>
        /// Gets the synchronize table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public IMobileServiceSyncTable<T> GetSyncTable<T>() => _mobileServiceClient.GetSyncTable<T>();

        /// <inheritdoc />
        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public IMobileServiceTable GetTable(string tableName) => _mobileServiceClient.GetTable(tableName);

        /// <inheritdoc />
        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMobileServiceTable<T> GetTable<T>() => _mobileServiceClient.GetTable<T>();

        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<T> InvokeApiAsync<T>(string apiName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync<T>(apiName, cancellationToken);

        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="body">The body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<U> InvokeApiAsync<T, U>(string apiName,
            T body,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync<T, U>(apiName, body, cancellationToken);

        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<T> InvokeApiAsync<T>(string apiName,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync<T>(apiName, method, parameters, cancellationToken);

        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="body">The body.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<U> InvokeApiAsync<T, U>(string apiName,
            T body,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync<T, U>(apiName, body, method, parameters, cancellationToken);

        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<JToken> InvokeApiAsync(string apiName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync(apiName, cancellationToken);

        /// <inheritdoc />
        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="body">The body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<JToken> InvokeApiAsync(string apiName,
            JToken body,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync(apiName, body, cancellationToken);

        /// <inheritdoc />
        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<JToken> InvokeApiAsync(string apiName,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync(apiName, method, parameters, cancellationToken);

        /// <inheritdoc />
        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="body">The body.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<JToken> InvokeApiAsync(string apiName,
            JToken body,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _mobileServiceClient.InvokeApiAsync(apiName, body, method, parameters, cancellationToken);

        /// <inheritdoc />
        /// <summary>
        /// Invokes the API asynchronous.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="content">The content.</param>
        /// <param name="method">The method.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> InvokeApiAsync(string apiName,
            HttpContent content,
            HttpMethod method,
            IDictionary<string, string> requestHeaders,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) => await _mobileServiceClient.InvokeApiAsync(
            apiName,
            content,
            method,
            requestHeaders,
            parameters,
            cancellationToken);

        /// <inheritdoc />
        /// <summary>
        /// Logins the asynchronous.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public virtual Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, JObject token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logins the asynchronous.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<MobileServiceUser> LoginAsync(string provider, JObject token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logouts the asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Refreshes the user asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<MobileServiceUser> RefreshUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}