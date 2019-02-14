using System.Threading.Tasks;
using Rocket.Surgery.Azure;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Azure.Sync.Tests
{
    internal class TestSyncOperation : SyncOperation
    {
        public override async Task<ISyncOperationResponse> Execute(bool pushPendingTransactions = false) =>
            await Task.FromResult(new SyncOperationResponse());
    }
}