using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using NSubstitute;
using Rocket.Surgery.Azure.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Azure.Sync.Tests
{
    internal class BaseStoreFixture<T> : ITestFixtureBuilder
        where T : class, IDataAccessObject, new()
    {
        private IMobileServiceSyncTable<T> _syncTable;
        private IMobileServiceTable<T> _serviceTable;
        private IStoreService _storageService;
        private IMobileServiceTableQuery<T> _mobileServiceTableQuery;

        public IStoreService StorageService { get; set; }

        public IMobileServiceSyncTable<T> SyncTable { get; set; }

        public BaseStoreFixture()
        {
            _storageService = Substitute.For<IStoreService>();
            _syncTable = Substitute.For<IMobileServiceSyncTable<T>>();
            _serviceTable = Substitute.For<IMobileServiceTable<T>>();
            _mobileServiceTableQuery = Substitute.For<IMobileServiceTableQuery<T>>();
        }

        public BaseStoreFixture<T> WithSyncTable(TestTable<T> table) => this.With(ref _syncTable, table);

        public BaseStoreFixture<T> WithSyncTable(IMobileServiceSyncTable<T> table) => this.With(ref _syncTable, table);

        public BaseStoreFixture<T> WithSyncTable() => this.With(ref _syncTable, new TestTable<T>());

        public BaseStoreFixture<T> WithTable(IMobileServiceTable<T> table) => this.With(ref _serviceTable, table);

        public BaseStoreFixture<T> WithStorageService(IStoreService storageService) =>
            this.With(ref _storageService, storageService);

        public static implicit operator BaseStore<T>(BaseStoreFixture<T> fixture) => fixture.Build();

        private BaseStore<T> Build()
        {
            _storageService.GetSyncTable<T>().Returns(_syncTable);
            _storageService.GetTable<T>().Returns(_serviceTable);
            return new BaseStore<T>(_storageService);
        }
    }
}