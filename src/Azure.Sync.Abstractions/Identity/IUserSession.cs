using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Interface representing a users session information.
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is authenticated.
        /// </summary>
        bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [requires password update].
        /// </summary>
        bool RequiresPasswordUpdate { get; set; }

        /// <summary>
        /// Gets or sets the token expiration.
        /// </summary>
        DateTimeOffset TokenExpiration { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        string UserName { get; set; }
    }
}