using Rocket.Surgery.Azure.Sync.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Azure
{
    public class SyncService : ISyncService
    {
        /// <summary>
        /// The synchronize operations
        /// </summary>
        protected readonly IEnumerable<ISyncOperation> SyncOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sync" /> class.
        /// </summary>
        /// <param name="syncOperations">The synchronize operations.</param>
        public SyncService(IEnumerable<ISyncOperation> syncOperations) { SyncOperations = syncOperations; }

        /// <inheritdoc />
        public IEnumerable<ISyncOperation> GetAll() => SyncOperations;

        /// <inheritdoc />
        public IEnumerable<ISyncOperation> GetInitial() => SyncOperations.Where(x => x.Initial);
    }
}