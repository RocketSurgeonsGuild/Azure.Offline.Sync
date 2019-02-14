using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions.Store
{
    /// <summary>
    /// Interface representation of a mobile synce data transport object.
    /// </summary>
    public interface IDataAccessObject
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the resource access identifier.
        /// </summary>
        long ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the created at <see cref="DateTimeOffset"/>.
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IDataAccessObject"/> is deleted.
        /// </summary>
        bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        string Version { get; set; }
    }
}