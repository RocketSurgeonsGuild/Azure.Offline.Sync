using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure
{
    public class SyncOperationResponse : ISyncOperationResponse
    {
        /// <inheritdoc />
        public bool IsSuccessful { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }
    }
}