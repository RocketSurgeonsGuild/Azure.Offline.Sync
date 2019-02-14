using System;
using System.Threading.Tasks;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure
{
    public abstract class SessionSyncOperation : SyncOperation, ISessionSyncOperation
    {
        public async Task<ISyncOperationResponse> Execute(ISyncOperationRequest request,
            bool pushPendingTransactions = false) => await Task.FromResult(default(ISyncOperationResponse));
    }
}