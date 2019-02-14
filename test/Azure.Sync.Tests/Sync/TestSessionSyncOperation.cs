using System.Threading.Tasks;
using Rocket.Surgery.Azure;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Azure.Sync.Tests
{
    internal class TestSessionSyncOperation : SyncOperation, ISessionSyncOperation
    {
        public override async Task<ISyncOperationResponse> Execute(bool pushPendingTransactions = false) =>
            await Task.FromResult(new SyncOperationResponse());

        public async Task<ISyncOperationResponse> Execute(ISyncOperationRequest request, bool pushPendingTransactions = false) =>
            await Task.FromResult(new SyncOperationResponse());
    }
}