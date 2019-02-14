using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// A <see cref="ISyncOperation" /> that gets context requests.
    /// </summary>
    /// <seealso cref="ISyncOperation" />
    public interface ISyncOperation
    {
        /// <summary>
        /// Gets or sets where this sync operation should be performed during the initial sync.
        /// </summary>
        bool Initial { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the maximum attempts for a sync operation.
        /// </summary>
        int MaxAttempts { get; }

        /// <summary>
        /// Executes the sync operation.
        /// </summary>
        /// <param name="pushPendingTransactions">if set to <c>true</c> [push pending transactions].</param>
        /// <returns></returns>
        Task<ISyncOperationResponse> Execute(bool pushPendingTransactions = false);
    }
}