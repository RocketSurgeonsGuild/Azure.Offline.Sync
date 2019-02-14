using System.Reactive.Linq;
using System.Threading.Tasks;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure
{
    public abstract class SyncOperation : ISyncOperation
    {
        /// <inheritdoc />
        public bool Initial { get; set; } = true;

        /// <inheritdoc />
        public virtual string Name => "Sync";

        /// <inheritdoc />
        public int MaxAttempts => 1;

        /// <inheritdoc />
        public abstract Task<ISyncOperationResponse> Execute(bool pushPendingTransactions = false);
    }
}