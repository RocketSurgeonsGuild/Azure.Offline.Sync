using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure.Sync
{
    public class ZumoJwtUtility : IJwtUtility
    {
        private static DateTime UnixEpoch => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc />
        public string DecodePayload(string tokenPayload)
        {
            var payload = tokenPayload.Length % 4 == 1;
            if (payload)
            {
                throw new InvalidTokenException("Invalid user token");
            }

            //Replace 62nd and 63rd char of encoding
            tokenPayload = tokenPayload.Replace("-", "+");
            tokenPayload = tokenPayload.Replace("_", "/");

            // Add padding:
            var padding = 4 - tokenPayload.Length % 4;
            tokenPayload = tokenPayload.PadRight(tokenPayload.Length + padding % 4, '=');

            var tokenPayloadBytes = Convert.FromBase64String(tokenPayload);

            return Encoding.UTF8.GetString(tokenPayloadBytes, 0, tokenPayloadBytes.Length);
        }

        /// <inheritdoc />
        public T GetToken<T>(string rawToken) where T : IJwt
        {
            var tokenParts = rawToken.Split('.');

            if (tokenParts.Length != 3)
            {
                throw new InvalidTokenException("Invalid user token");
            }

            var mobileAppToken = DecodePayload(tokenParts.Skip(1).First());
            return JsonConvert.DeserializeObject<T>(mobileAppToken);
        }

        /// <inheritdoc />
        public DateTime? GetTokenExpiration<T>(string token)
            where T : IJwt
        {
            var claims = GetToken<T>(token);

            return (UnixEpoch + TimeSpan.FromSeconds(claims.Expiration)).ToUniversalTime();
        }
    }
}