using Microsoft.WindowsAzure.MobileServices;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Azure.Sync
{
    public interface IMobileServiceClientFactory
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetClient<T>() where T : IMobileServiceClient;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        T GetClient<T>(params object[] parameters) where T : IMobileServiceClient;
    }
}