using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Store
{
    /// <summary>
    /// Interface representing a base data store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IBaseStore" />
    public interface IBaseStore<T> : IBaseStore
        where T : IDataAccessObject
    {
        /// <summary>
        /// Gets the identifier for the <see cref="IBaseStore"/>.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the store service.
        /// </summary>
        IStoreService StoreService { get; }

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <returns></returns>
        void DropTable();

        /// <summary>
        /// Gets the item asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="forceRefresh">if set to <c>true</c> [force refresh].</param>
        /// <returns></returns>
        Task<T> GetItemAsync(string id, bool forceRefresh = false);

        /// <summary>
        /// Gets the items asynchronously from the database.
        /// </summary>
        /// <param name="forceRefresh">if set to <c>true</c> [force refresh].</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        /// <summary>
        /// Initializes the store.
        /// </summary>
        /// <returns></returns>
        Task InitializeStore();

        /// <summary>
        /// Inserts the specified item to the database asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task InsertAsync(T item);

        /// <summary>
        /// Inserts the specified item online asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task InsertOnlineAsync(T item);

        /// <summary>
        /// Purges the database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task PurgeAsync();

        /// <summary>
        /// Removes the item from the database asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task RemoveAsync(T item);

        /// <summary>
        /// Removes the item online asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task RemoveOnlineAsync(T item);

        /// <summary>
        /// Synchronizes database with the online store the asynchronously.
        /// </summary>
        /// <param name="pushPendingTransactions">if set to <c>true</c> [push pending transactions].</param>
        /// <returns></returns>
        Task SyncAsync(bool pushPendingTransactions = true);

        /// <summary>
        /// Updates the item in the database asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task UpdateAsync(T item);

        /// <summary>
        /// Updates the item online asynchronously.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task UpdateOnlineAsync(T item);

        /// <summary>
        /// Pulls the latest asynchronously.
        /// </summary>
        /// <returns></returns>
        Task PullLatestAsync();
    }

    /// <summary>
    /// Interface representation for a base <see cref="IDataAccesObject"/> storage location.
    /// </summary>
    /// <seealso cref="IBaseStore" />
    public interface IBaseStore { }
}