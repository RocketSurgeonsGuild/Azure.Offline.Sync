using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Rocket.Surgery.Azure.Sync
{
    /// <summary>
    /// Base azure table store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IBaseStore{T}" />
    public class BaseStore<T> : IBaseStore<T> where T : class, IDataAccessObject, new()
    {
        private readonly PullOptions _options = new PullOptions { MaxPageSize = 1000 };
        private IMobileServiceTable<T> _onlineTable;
        private IMobileServiceSyncTable<T> _table;

        /// <inheritdoc />
        public virtual string Identifier => "base";

        /// <summary>
        /// Gets the online table.
        /// </summary>
        public IMobileServiceTable<T> OnlineTable => _onlineTable ?? (_onlineTable = StoreService.GetTable<T>());

        /// <inheritdoc />
        public IStoreService StoreService { get; }

        /// <summary>
        /// Gets the table.
        /// </summary>
        public IMobileServiceSyncTable<T> Table => _table ?? (_table = StoreService.GetSyncTable<T>());

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStore{T}"/> class.
        /// </summary>
        /// <param name="storeService">The store service.</param>
        public BaseStore(IStoreService storeService)
        {
            StoreService = storeService;
        }

        /// <inheritdoc />
        public virtual void DropTable()
        {
            _table = null;
            _onlineTable = null;
        }

        /// <inheritdoc />
        public async Task<T> GetItemAsync(string id, bool forceRefresh = false)
        {
            await InitializeStore();
            if (forceRefresh)
            {
                await PullLatestAsync().ContinueOnAnyContext();
            }

            var items = await Table.Where(item => item.Id == id).ToListAsync();

            if (!items?.Any() ?? true)
            {
                return null;
            }

            return items.First();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            await InitializeStore();
            if (forceRefresh)
            {
                await PullLatestAsync().ContinueOnAnyContext();
            }

            return await Table.ToEnumerableAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public async Task InitializeStore()
        {
            if (StoreService != null)
            {
                if (!StoreService.IsInitialized)
                {
                    await StoreService.InitializeAsync().ContinueOnAnyContext();
                }
            }
            else
            {
                throw new Exception(
                    $"Store Service is null in {typeof(T).Name}.InitializeStore - check container registration");
            }
        }

        /// <inheritdoc />
        public virtual async Task InsertAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await PullLatestAsync().ContinueOnAnyContext();
            await Table.InsertAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public virtual async Task InsertOnlineAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await OnlineTable.InsertAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public async Task PullLatestAsync()
        {
            try
            {
                await Table.PullAsync($"all{Identifier}", Table.CreateQuery(), true, CancellationToken.None, _options).ContinueOnAnyContext();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to pull items, that is alright as we have offline capabilities: " + ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task PurgeAsync()
        {
            try
            {
                await Table.PurgeAsync($"all{Identifier}", Table.CreateQuery(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to purge items: " + ex);
            }
        }

        /// <inheritdoc />
        public virtual async Task RemoveAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await PullLatestAsync().ContinueOnAnyContext();
            await Table.DeleteAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public virtual async Task RemoveOnlineAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await PullLatestAsync().ContinueOnAnyContext();
            await OnlineTable.DeleteAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public virtual async Task SyncAsync(bool pushPendingTransactions = true)
        {
            try
            {
                if (pushPendingTransactions)
                {
                    await StoreService.SyncContext.PushAsync(CancellationToken.None).ContinueOnAnyContext();
                }

                await PullLatestAsync().ContinueOnAnyContext();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync items, that is alright as we have offline capabilities: " + ex);
            }
            finally
            {
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await Table.UpdateAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }

        /// <inheritdoc />
        public virtual async Task UpdateOnlineAsync(T item)
        {
            await InitializeStore().ContinueOnAnyContext();
            await OnlineTable.UpdateAsync(item).ContinueOnAnyContext();
            await SyncAsync().ContinueOnAnyContext();
        }
    }
}