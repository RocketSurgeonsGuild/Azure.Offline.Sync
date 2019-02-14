using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        IAuthenticationSession CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the token expiration.
        /// </summary>
        DateTimeOffset TokenExpiration { get; set; }
    }
}