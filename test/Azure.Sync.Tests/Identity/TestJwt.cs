using System.Collections.Generic;
using Rocket.Surgery.Azure.Sync;

namespace Azure.Sync.Tests
{
    public class TestJwt : ZumoJwt
    {
        public string Organization { get; set; }

        public string Role { get; set; }

        public IEnumerable<string> Facilities { get; set; }
    }
}