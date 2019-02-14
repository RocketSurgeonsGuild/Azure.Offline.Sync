using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using NSubstitute;
using Rocket.Surgery.Azure.Sync;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class AzureStoreServiceTests
    {
        public class TheDatabaseProperty
        {
            [Fact]
            public async Task Should_Call_Sync_Context()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                await sut.Database.InitializeAsync();

                // Then
                await client.SyncContext.Store.Received().InitializeAsync();
            }
        }

        public class TheGetSyncTableMethod
        {
            [Fact]
            public void Should_Call_Decorated()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                sut.GetSyncTable<TestObjects>();

                // Then
                client.Received().GetSyncTable<TestObjects>();
            }
        }

        public class TheGetTableMethod
        {
            [Fact]
            public void Should_Call_Decorated()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                sut.GetTable<TestObjects>();

                // Then
                client.Received().GetTable<TestObjects>();
            }
        }

        public class TheInitializeMethod
        {
            public async Task Should_Call_Decorated()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                await sut.InitializeAsync();

                // Then
                client.Received().GetSyncTable<TestObjects>();
            }
        }

        public class TheDropEverythingMethod
        {
            public async Task Should_Call_Decorated()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                await sut.DropEverythingAsync();

                // Then
                client.Received().GetSyncTable<TestObjects>();
            }
        }

        public class TheRefreshUserMethod
        {
            [Fact]
            public async Task Should_Call_Decorated()
            {
                // Given
                var client = Substitute.For<IMobileServiceClient>();
                AzureStoreService sut = new AzureStoreServiceFixture().WithClient(client);

                // When
                await sut.RefreshUserAsync();

                // Then
                await client.Received().RefreshUserAsync();
            }
        }
    }
}
