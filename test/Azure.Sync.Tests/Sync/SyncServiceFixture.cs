using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Azure;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Azure.Sync.Tests
{
    internal class SyncServiceFixture : ITestFixtureBuilder
    {
        private IEnumerable<ISyncOperation> _syncOperations;

        public SyncServiceFixture() { _syncOperations = Enumerable.Empty<ISyncOperation>(); }

        public SyncServiceFixture WithOperations(IEnumerable<ISyncOperation> operations) =>
            this.With(ref _syncOperations, operations);

        public static implicit operator SyncService(SyncServiceFixture fixture) =>
            fixture.Build();

        private SyncService Build() { return new SyncService(_syncOperations); }
    }
}