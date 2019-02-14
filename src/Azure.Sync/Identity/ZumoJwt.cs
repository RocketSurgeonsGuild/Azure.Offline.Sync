using System;
using Newtonsoft.Json;
using Rocket.Surgery.Azure.Sync.Abstractions;

namespace Rocket.Surgery.Azure.Sync
{
    public abstract class ZumoJwt : IZumoJwt
    {
        /// <inheritdoc />
        [JsonProperty("aud")]
        public string Audience { get; set; }

        /// <inheritdoc />
        [JsonProperty("exp")]
        public long Expiration { get; set; }

        /// <inheritdoc />
        [JsonProperty("iss")]
        public string Issuer { get; set; }

        /// <inheritdoc />
        [JsonProperty("nbf")]
        public int Nbf { get; set; }

        /// <inheritdoc />
        [JsonProperty("sub")]
        public string Subject { get; set; }

        /// <inheritdoc />
        [JsonProperty("ver")]
        public string Version { get; set; }
    }
}