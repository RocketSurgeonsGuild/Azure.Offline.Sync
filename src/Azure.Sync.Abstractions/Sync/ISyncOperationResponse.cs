namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Interface representing a response of a sync operation.
    /// </summary>
    public interface ISyncOperationResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is successful.
        /// </summary>
        bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }
    }
}