using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Rocket.Surgery.Azure.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Azure.Sync.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class AzureStoreServiceBenchmark
    {
        private IMobileServiceClient _mobileServiceClient;
        private IMobileServiceLocalStore _mobileServiceLocalStore;
        private ISqlConnection _sqlConnection;
        private AzureStoreService _azureStoreService;
        private const string CLIENT_URL = "https://offline.sync.com";

        /// <summary>
        /// Setups this benchmark.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _mobileServiceClient = new AzureMobileServiceClient(new MobileServiceClient(CLIENT_URL));
            _mobileServiceLocalStore = new MockServiceLocalStore();
            _sqlConnection = new MockSqlConnection();
        }

        /// <summary>
        /// Sets ups the iteration.
        /// </summary>
        [IterationSetup]
        public void SetupIteration()
        {
            _azureStoreService = new AzureStoreService(_mobileServiceClient, _mobileServiceLocalStore);
            _azureStoreService.CurrentUser = new MobileServiceUser(CLIENT_URL);
        }

        /// <summary>
        /// Cleans ups the iteration.
        /// </summary>
        [IterationCleanup]
        public void CleanupIteration()
        {
            _azureStoreService = null;
        }

        /// <summary>
        /// Benchmarks the creation of <see cref="AzureStoreService"/>.
        /// </summary>
        /// <returns>The azure store service.</returns>
        [Benchmark(Baseline = true)]
        public AzureStoreService Create() => new AzureStoreService(_mobileServiceClient, _mobileServiceLocalStore);

        /// <summary>
        /// Benchmarks the creation of <see cref="IMobileServiceClient"/>.
        /// </summary>
        /// <returns>The mobile service client.</returns>
        [Benchmark]
        public IMobileServiceClient Client() => _azureStoreService.Client;

        /// <summary>
        /// Benchmarks the creation of <see cref="MobileServiceUser"/>.
        /// </summary>
        /// <returns>The mobile service user.</returns>
        [Benchmark]
        public MobileServiceUser CurrentUser() => _azureStoreService.CurrentUser;
    }
}
