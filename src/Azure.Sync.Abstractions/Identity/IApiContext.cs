namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Interface representing an api context.
    /// </summary>
    public interface IApiContext
    {
        /// <summary>
        /// Gets or sets the minimum API version.
        /// </summary>
        int MinimumApiVersion { get; set; }
    }
}