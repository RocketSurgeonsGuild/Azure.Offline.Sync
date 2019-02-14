using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Rocket.Surgery.Azure.Sync;

namespace Azure.Sync.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class AzureMobileServiceClientBenchmark
    {
        private AzureMobileServiceClient _mobileServiceClient;
        private const string CLIENT_URL = "https://offline.sync.com";

        [GlobalSetup]
        public void Setup()
        {
            _mobileServiceClient = new AzureMobileServiceClient(new MobileServiceClient(CLIENT_URL));
        }

        [Benchmark(Baseline = true)]
        public AzureMobileServiceClient Create() => new AzureMobileServiceClient(new MobileServiceClient(CLIENT_URL));

        [Benchmark]
        public IMobileServiceTable GetTable() => _mobileServiceClient.GetTable("table");

        [Benchmark]
        public IMobileServiceSyncTable GetSyncTable() => _mobileServiceClient.GetSyncTable("synctable");
    }
}
