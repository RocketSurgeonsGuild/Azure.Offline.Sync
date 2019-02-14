using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Eventing;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using Rocket.Surgery.Azure.Sync.Handlers;
using Rocket.Surgery.Azure.Sync.SQLite;

namespace Rocket.Surgery.Azure.Sync
{
    /// <summary>
    /// The base store service Decorator
    /// </summary>
    /// <seealso cref="IStoreService" />
    public class AzureStoreService : IStoreService
    {
        private readonly IMobileServiceClient _client;
        private readonly IMobileServiceLocalStore _localStore;

        /// <inheritdoc />
        public Uri AlternateLoginHost
        {
            get => _client.AlternateLoginHost;
            set => _client.AlternateLoginHost = value;
        }

        /// <inheritdoc />
        public IMobileServiceClient Client => _client;

        /// <inheritdoc />
        public MobileServiceUser CurrentUser
        {
            get => _client.CurrentUser;
            set => _client.CurrentUser = value;
        }

        /// <inheritdoc />
        public IMobileServiceLocalStore Database => _client.SyncContext.Store;

        /// <inheritdoc />
        public string LoginUriPrefix
        {
            get => _client.LoginUriPrefix;
            set => _client.LoginUriPrefix = value;
        }

        /// <inheritdoc />
        public MobileServiceJsonSerializerSettings SerializerSettings
        {
            get => _client.SerializerSettings;
            set => _client.SerializerSettings = value;
        }

        /// <inheritdoc />
        public IMobileServiceEventManager EventManager => _client.EventManager;

        /// <inheritdoc />
        public string InstallationId => _client.InstallationId;

        /// <inheritdoc />
        public bool IsInitialized => _client.SyncContext.IsInitialized;

        /// <inheritdoc />
        public Uri MobileAppUri => _client.MobileAppUri;

        /// <inheritdoc />
        public IMobileServiceSyncContext SyncContext => _client.SyncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStoreService" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="localStore">The SQL connection.</param>
        public AzureStoreService(IMobileServiceClient client, IMobileServiceLocalStore localStore)
        {
            _client = client;
            _localStore = localStore;

            var converter = _client
                                ?.SerializerSettings
                                ?.Converters
                                .FirstOrDefault(c => c is MobileServiceIsoDateTimeConverter);

            if (converter != null)
            {
                _client.SerializerSettings.Converters.Remove(converter);
            }

            _client?.SerializerSettings?.Converters.Add(new AzureIsoDateTimeConverter());
        }

        /// <inheritdoc />
        public async Task DropEverythingAsync()
        {
            // purge all data from all tables.
            await PurgeTables();

            // reset all tables back to initial state.
            await ResetTables();
        }

        /// <inheritdoc />
        public IMobileServiceSyncTable GetSyncTable(string tableName) => _client.GetSyncTable(tableName);

        /// <inheritdoc />
        public IMobileServiceSyncTable<T> GetSyncTable<T>() => _client.GetSyncTable<T>();

        /// <inheritdoc />
        public IMobileServiceTable GetTable(string tableName) => _client.GetTable(tableName);

        /// <inheritdoc />
        public IMobileServiceTable<T> GetTable<T>() => _client.GetTable<T>();

        /// <summary>
        /// Initializes the database asynchronously.
        /// override this method to initialize individual stores.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public virtual async Task InitializeAsync()
        {
            await _client.SyncContext.InitializeAsync(_localStore, new LoggingHandler()).ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public async Task<T> InvokeApiAsync<T>(string apiName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync<T>(apiName, cancellationToken);

        /// <inheritdoc />
        public async Task<U> InvokeApiAsync<T, U>(string apiName,
            T body,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync<T, U>(apiName, body, cancellationToken);

        /// <inheritdoc />
        public async Task<T> InvokeApiAsync<T>(string apiName,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync<T>(apiName, method, parameters, cancellationToken);

        /// <inheritdoc />
        public async Task<U> InvokeApiAsync<T, U>(string apiName,
            T body,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync<T, U>(apiName, body, method, parameters, cancellationToken);

        /// <inheritdoc />
        public async Task<JToken> InvokeApiAsync(string apiName,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync(apiName, cancellationToken);

        /// <inheritdoc />
        public async Task<JToken> InvokeApiAsync(string apiName,
            JToken body,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync(apiName, body, cancellationToken);

        /// <inheritdoc />
        public async Task<JToken> InvokeApiAsync(string apiName,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync(apiName, method, parameters, cancellationToken);

        /// <inheritdoc />
        public async Task<JToken> InvokeApiAsync(string apiName,
            JToken body,
            HttpMethod method,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync(apiName, body, method, parameters, cancellationToken);

        /// <inheritdoc />
        public async Task<HttpResponseMessage> InvokeApiAsync(string apiName,
            HttpContent content,
            HttpMethod method,
            IDictionary<string, string> requestHeaders,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = new CancellationToken()) =>
            await _client.InvokeApiAsync(apiName,
                content,
                method,
                requestHeaders,
                parameters,
                cancellationToken);

        /// <inheritdoc />
        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, JObject token) => await _client.LoginAsync(provider, token);

        /// <inheritdoc />
        public async Task<MobileServiceUser> LoginAsync(string provider, JObject token) => await _client.LoginAsync(provider, token);

        /// <inheritdoc />
        public async Task LogoutAsync() => await _client.LogoutAsync();

        /// <inheritdoc />
        public async Task<MobileServiceUser> RefreshUserAsync()
        {
            return await _client.RefreshUserAsync();
        }

        /// <inheritdoc />
        public virtual Task ResetTables()
        {
            // Drop All Tables
            // Store.DropTable()
            return Task.CompletedTask;
        }

        /// <summary>
        /// Synchronizes the database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SyncAllAsync()
        {
            if (!IsInitialized)
            {
                await InitializeAsync();
            }

            var taskList = new List<Task<bool>>();

            var successes = await Task.WhenAll(taskList).ContinueOnAnyContext();
            return successes.Any(x => !x); //if any of the tasks failed
        }

        /// <summary>
        /// Purges the tables.
        /// </summary>
        /// <returns></returns>
        protected virtual Task PurgeTables()
        {
            // Purge All Tables
            // Store.PurgeAsync();
            return Task.CompletedTask;
        }
    }
}