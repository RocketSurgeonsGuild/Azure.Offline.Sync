using System.Collections.Generic;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Represents a service that provides <see cref="ISessionSyncOperation"/>.
    /// </summary>
    /// <seealso cref="ISyncService" />
    public interface ISessionSync : ISyncService
    {
        /// <summary>
        /// Gets the user specific synchronize operations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISessionSyncOperation> GetSession();
    }
}