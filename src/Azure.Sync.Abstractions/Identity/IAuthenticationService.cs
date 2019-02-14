using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<IAuthenticationResponse> Authenticate(IAuthenticationRequest request);

        /// <summary>
        /// Deserializes the token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="authToken">The authentication token.</param>
        /// <returns></returns>
        T DeserializeToken<T>(string authToken) where T : IJwt;

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        void Logout();
    }
}