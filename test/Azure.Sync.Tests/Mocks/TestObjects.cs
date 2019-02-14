using System;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Rocket.Surgery.Azure.Sync.Abstractions.Store;

namespace Azure.Sync.Tests
{
    public class TestObjects : IDataAccessObject
    {
        public DateTimeOffset CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public string Id { get; set; }
        public long ResourceId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Version { get; set; }
    }
}