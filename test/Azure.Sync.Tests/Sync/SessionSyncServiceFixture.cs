using System.Collections.Generic;
using System.Linq;
using Rocket.Surgery.Azure;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Azure.Sync.Tests
{
    internal class SessionSyncServiceFixture : ITestFixtureBuilder
    {
        private IEnumerable<ISyncOperation> _syncOperations;

        public SessionSyncServiceFixture()
        {
            _syncOperations = new List<ISyncOperation>
            {
                new TestSessionSyncOperation(),
                new TestSyncOperation()
            };
        }

        public SessionSyncServiceFixture WithOperations(IEnumerable<ISyncOperation> operations) =>
            this.With(ref _syncOperations, operations);

        public static implicit operator SessionSyncService(SessionSyncServiceFixture fixture) =>
            fixture.Build();

        private SessionSyncService Build() => new SessionSyncService(_syncOperations);
    }
}