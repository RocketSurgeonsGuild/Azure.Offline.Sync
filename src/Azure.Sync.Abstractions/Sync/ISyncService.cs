using System.Collections.Generic;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Represents a service that provides <see cref="ISyncOperation"/>.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Gets the complete synchronize operations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISyncOperation> GetAll();

        /// <summary>
        /// Gets the initial synchronize operations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISyncOperation> GetInitial();
    }
}