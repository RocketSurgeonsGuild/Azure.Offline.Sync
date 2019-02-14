using NSubstitute;
using Rocket.Surgery.Azure.Sync.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Rocket.Surgery.Azure.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class BaseStoreTests
    {
        public class TheInitializeStoreMethod
        {
            [Fact]
            public async Task Should_Throw_If_Store_Null()
            {
                // Given
                BaseStore<TestObjects> fixture = new BaseStore<TestObjects>(null);

                // When
                var result = await Record.ExceptionAsync(async () => await fixture.InitializeStore());

                // Then
                result.Message.Should().Contain("Store Service");
            }

            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.InitializeStore();

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Not_Throw_If_Store_Not_Initialized()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                var result = await Record.ExceptionAsync(async () => await fixture.InitializeStore());

                // Then
                result.Should().BeNull();
            }
        }

        public class TheInsertAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.InsertAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Insert_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.InsertAsync(new TestObjects());

                // Then
                await table.Received().InsertAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Call_Pull_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.InsertAsync(new TestObjects());

                // Then
                await table.Received().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    true,
                    Arg.Any<CancellationToken>(),
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.InsertAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }

        public class TheInsertOnlineAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.InsertOnlineAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Insert_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithTable(table);

                // When
                await fixture.InsertOnlineAsync(new TestObjects());

                // Then
                await table.Received().InsertAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.InsertOnlineAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }

        public class TheGetItemAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Get_Sync_Table()
            {
                // Given
                var storeService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storeService);

                // When
                await fixture.GetItemAsync("1");

                // Then
                storeService.Received().GetSyncTable<TestObjects>();
            }

            [Fact]
            public async Task Should_Call_Initialize_Store()
            {
                // Given
                var storeService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storeService);

                // When
                await fixture.GetItemAsync("1");

                // Then
                await storeService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Pull_Async()
            {
                // Given
                var syncTable = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(syncTable);

                // When
                await fixture.GetItemAsync("1", true);

                // Then
                await syncTable.Received().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    Arg.Any<bool>(),
                    CancellationToken.None,
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Call_Table()
            {
                // Given
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>();

                // When
                await fixture.GetItemAsync("1", true);

                // Then
                fixture.Table.Received();
            }
        }

        public class TheGetItemsAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Get_Sync_Table()
            {
                // Given
                var storeService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storeService);

                // When
                await fixture.GetItemsAsync();

                // Then
                storeService.Received().GetSyncTable<TestObjects>();
            }

            [Fact]
            public async Task Should_Call_Initialize_Store()
            {
                // Given
                var storeService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storeService);

                // When
                await fixture.GetItemsAsync();

                // Then
                await storeService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Pull_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.GetItemsAsync(true);

                // Then
                await table.Received().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    Arg.Any<bool>(),
                    CancellationToken.None,
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Not_Call_Pull_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.GetItemsAsync();

                // Then
                await table.DidNotReceive().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    Arg.Any<bool>(),
                    CancellationToken.None,
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Call_Table()
            {
                // Given
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>();

                // When
                await fixture.GetItemsAsync(true);

                // Then
                fixture.Table.Received();
            }
        }

        public class TheUpdateAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.UpdateAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Update_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.UpdateAsync(new TestObjects());

                // Then
                await table.Received().UpdateAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Call_Pull_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.UpdateAsync(new TestObjects());

                // Then
                await table.Received().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    true,
                    Arg.Any<CancellationToken>(),
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.UpdateAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }

        public class TheUpdateOnlineAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.UpdateOnlineAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Update_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithTable(table);

                // When
                await fixture.UpdateOnlineAsync(new TestObjects());

                // Then
                await table.Received().UpdateAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.UpdateOnlineAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }

        public class TheRemoveAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.RemoveAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Insert_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.RemoveAsync(new TestObjects());

                // Then
                await table.Received().DeleteAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Call_Pull_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceSyncTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithSyncTable(table);

                // When
                await fixture.RemoveAsync(new TestObjects());

                // Then
                await table.Received().PullAsync(Arg.Any<string>(),
                    Arg.Any<IMobileServiceTableQuery<TestObjects>>(),
                    true,
                    Arg.Any<CancellationToken>(),
                    Arg.Any<PullOptions>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.RemoveAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }

        public class TheRemoveOnlineAsyncMethod
        {
            [Fact]
            public async Task Should_Call_Initialize()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                storageService.IsInitialized.Returns(false);
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.RemoveOnlineAsync(new TestObjects());

                // Then
                await storageService.Received().InitializeAsync();
            }

            [Fact]
            public async Task Should_Call_Delete_Async()
            {
                // Given
                var table = Substitute.For<IMobileServiceTable<TestObjects>>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithTable(table);

                // When
                await fixture.RemoveOnlineAsync(new TestObjects());

                // Then
                await table.Received().DeleteAsync(Arg.Any<TestObjects>());
            }

            [Fact]
            public async Task Should_Push_Sync_Context()
            {
                // Given
                var storageService = Substitute.For<IStoreService>();
                BaseStore<TestObjects> fixture = new BaseStoreFixture<TestObjects>().WithStorageService(storageService);

                // When
                await fixture.RemoveOnlineAsync(new TestObjects());

                // Then
                await storageService.SyncContext.Received().PushAsync(CancellationToken.None);
            }
        }
    }
}