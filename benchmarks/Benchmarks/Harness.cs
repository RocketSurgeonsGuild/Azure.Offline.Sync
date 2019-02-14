using System;
using BenchmarkDotNet.Running;
using Xunit;

namespace Azure.Sync.Benchmarks
{
    public class Harness
    {
        /// <summary>
        /// Azure store service benchmarks.
        /// </summary>
        [Fact]
        public void Azure_Store_Service()
        {
            BenchmarkRunner.Run<AzureStoreServiceBenchmark>();
        }

        /// <summary>
        /// Azure mobile service client benchmarks.
        /// </summary>
        [Fact]
        public void Azure_Mobile_Client()
        {
            BenchmarkRunner.Run<AzureMobileServiceClientBenchmark>();
        }

        /// <summary>
        /// Zumo JWT utility benchmarks.
        /// </summary>
        [Fact]
        public void Zumo_Jwt_Utility()
        {
            BenchmarkRunner.Run<ZumoJwtUtilityBenchmark>();
        }
    }
}
