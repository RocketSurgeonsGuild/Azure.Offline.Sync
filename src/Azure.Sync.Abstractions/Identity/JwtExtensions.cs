using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public static class JwtExtensions
    {
        /// <summary>
        /// Ases the claims.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static IEnumerable<IClaim> AsClaims<T>(this T token) where T : IJwt => Enumerable.Empty<IClaim>();
    }
}