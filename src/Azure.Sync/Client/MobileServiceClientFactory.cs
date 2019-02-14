using System;
using Microsoft.WindowsAzure.MobileServices;
using Rocket.Surgery.Azure.Sync.Abstractions.Azure.Sync;

namespace Rocket.Surgery.Azure.Sync
{
    internal class MobileServiceClientFactory : IMobileServiceClientFactory
    {
        /// <inheritdoc />
        public T GetClient<T>()
            where T : IMobileServiceClient
        {
            return Activator.CreateInstance<T>();
        }

        /// <inheritdoc />
        public T GetClient<T>(params object[] parameters)
            where T : IMobileServiceClient
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }
    }
}