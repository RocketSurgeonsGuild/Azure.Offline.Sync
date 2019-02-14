using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Interface representation of a JSON Web Token.
    /// </summary>
    public interface IJwt
    {
        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        string Audience { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        long Expiration { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the NBF.
        /// </summary>
        int Nbf { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        string Version { get; set; }
    }
}