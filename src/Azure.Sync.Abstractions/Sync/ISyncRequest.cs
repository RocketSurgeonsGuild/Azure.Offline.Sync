using System.Collections.Generic;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface ISyncRequest
    {
        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        IEnumerable<ISyncOperation> Operations { get; set; }
    }
}