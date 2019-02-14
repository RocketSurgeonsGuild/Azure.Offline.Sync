using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using NSubstitute;
using Rocket.Surgery.Azure.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Azure.Sync.Tests
{
    internal class AzureStoreServiceFixture : ITestFixtureBuilder
    {
        private IMobileServiceClient _client;
        private ISqlConnection _connection;
        private IMobileServiceLocalStore _store;
        private IMobileServiceSyncContext _context;

        public AzureStoreServiceFixture()
        {
            _client = Substitute.For<IMobileServiceClient>();
            _connection = Substitute.For<ISqlConnection>();
            _store = Substitute.For<IMobileServiceLocalStore>();
            _context = Substitute.For<IMobileServiceSyncContext>();
            _client.SyncContext.Returns(_context);
        }

        public AzureStoreServiceFixture WithClient(IMobileServiceClient client) => this.With(ref _client, client);

        public AzureStoreServiceFixture WithStore(IMobileServiceLocalStore store) => this.With(ref _store, store);

        public AzureStoreServiceFixture WithConnection(ISqlConnection sqlConnection) =>
            this.With(ref _connection, sqlConnection);

        public static implicit operator AzureStoreService(AzureStoreServiceFixture fixture) => fixture.Build();

        private AzureStoreService Build()
        {
            _client.SyncContext.Store.Returns(_store);

            return new AzureStoreService(_client, _store);
        }
    }
}