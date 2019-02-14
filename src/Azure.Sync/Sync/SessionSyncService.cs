using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure
{
    public class SessionSyncService : SyncService, ISessionSync
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionSyncService"/> class.
        /// </summary>
        /// <param name="syncOperations">The synchronize operations.</param>
        public SessionSyncService(IEnumerable<ISyncOperation> syncOperations) : base(syncOperations) { }


        /// <inheritdoc />
        public IEnumerable<ISessionSyncOperation> GetSession() =>
            SyncOperations
                .Where(x => x.GetType().GetInterfaces().Any(i => i == typeof(ISessionSyncOperation)))
                .Cast<ISessionSyncOperation>();
    }
}