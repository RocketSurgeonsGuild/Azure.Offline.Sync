using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// A <see cref="ISyncOperation" /> that gets user specific context requests.
    /// </summary>
    /// <seealso cref="ISyncOperation" />
    public interface ISessionSyncOperation : ISyncOperation
    {
        /// <summary>
        ///     Executes the sync operation specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="pushPendingTransactions">if set to <c>true</c> [push pending transactions].</param>
        /// <returns></returns>
        Task<ISyncOperationResponse> Execute(ISyncOperationRequest request, bool pushPendingTransactions = false);
    }
}