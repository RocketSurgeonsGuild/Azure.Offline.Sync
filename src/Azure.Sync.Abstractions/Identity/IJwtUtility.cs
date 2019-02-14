using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    /// <summary>
    /// Interface representation of a utility to parse <see cref="IJwt" /> types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJwtUtility
    {
        /// <summary>
        /// Decodes the payload and returns the underlying json.
        /// </summary>
        /// <param name="tokenPayload">The token payload.</param>
        /// <returns></returns>
        string DecodePayload(string tokenPayload);

        /// <summary>
        /// Gets the <see cref="IJwt"/> from the raw jwt string.
        /// </summary>
        /// <param name="rawToken">The raw token.</param>
        /// <returns></returns>
        T GetToken<T> (string rawToken) where T : IJwt;

        /// <summary>
        /// Gets the token expiration date for the jwt.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        DateTime? GetTokenExpiration<T>(string token) where T : IJwt;
    }
}