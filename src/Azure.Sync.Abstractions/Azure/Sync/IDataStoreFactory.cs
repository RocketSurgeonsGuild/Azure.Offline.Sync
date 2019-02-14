using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Azure.Sync
{
    public interface IDataStoreFactory
    {
        /// <summary>
        /// Gets the store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetStore<T>() where T : IBaseStore;

        /// <summary>
        /// Gets the store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        T GetStore<T>(params object[] parameters) where T : IBaseStore;
    }
}