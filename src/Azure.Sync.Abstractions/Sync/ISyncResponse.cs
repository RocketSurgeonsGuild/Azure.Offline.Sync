namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISyncResponse
    {
        /// <summary>
        /// Gets or sets the current task.
        /// </summary>
        string CurrentTask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is synchronize successful.
        /// </summary>
        bool IsSyncSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the next task.
        /// </summary>
        string NextTask { get; set; }
    }
}