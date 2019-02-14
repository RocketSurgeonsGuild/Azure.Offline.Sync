using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Store
{
    /// <inheritdoc />
    /// <summary>
    /// Interface representing an store service.
    /// </summary>
    /// <seealso cref="T:Microsoft.WindowsAzure.MobileServices.IMobileServiceClient" />
    public interface IStoreService : IMobileServiceClient
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        IMobileServiceClient Client { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        IMobileServiceLocalStore Database { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Drops everything asynchronously.
        /// </summary>
        /// <returns></returns>
        Task DropEverythingAsync();

        /// <summary>
        /// Initializes the database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// Resets the database tables.
        /// </summary>
        /// <returns></returns>
        Task ResetTables();

        /// <summary>
        /// Synchronizes the database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<bool> SyncAllAsync();
    }
}