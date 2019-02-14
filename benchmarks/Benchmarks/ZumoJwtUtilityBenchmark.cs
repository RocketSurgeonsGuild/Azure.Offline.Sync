using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Rocket.Surgery.Azure.Sync;

namespace Azure.Sync.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ZumoJwtUtilityBenchmark
    {
        private const string JwtString =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqZG9lIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJPcmdhbml6YXRpb24iOiIxMjM0NTYiLCJGYWNpbGl0aWVzIjpbIjEiLCIyIiwiMyIsIjQiXSwiUm9sZSI6IkFkbWluIiwiYXVkIjoiQWRtaW5pc3RyYXRvcnMiLCJleHAiOjE1MDAwLCJpc3MiOiJodHRwczovL2lzc3Vlci5jb20iLCJ2ZXIiOiIxLjIuMS45MDAifQ.PKDW7xpRR3oVRiQVKl3supFSIgaRRo1y2NjW-zx5V2A";

        /// <summary>
        /// The utility
        /// </summary>
        private ZumoJwtUtility Utility { get; set; }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            Utility = new ZumoJwtUtility();
        }

        /// <summary>
        /// Decodes the payload.
        /// </summary>
        [Benchmark]
        public string DecodePayload() => Utility.DecodePayload(JwtString.Split('.').Skip(1).First());

        /// <summary>
        /// Gets the token.
        /// </summary>
        [Benchmark]
        public ZumoJwt GetToken() => Utility.GetToken<MockJwt>(JwtString);

        /// <summary>
        /// Gets the token expiration.
        /// </summary>
        [Benchmark]
        public DateTime? GetTokenExpiration() => Utility.GetTokenExpiration<MockJwt>(JwtString);
    }
}