using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Rocket.Surgery.Azure;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class SyncServiceTests
    {
        public class TheGetAllMethod
        {
            [Fact]
            public void Should_Return_All()
            {
                // Given
                var operations = new List<ISyncOperation>
                {
                    new TestSyncOperation(),
                    new TestSyncOperation(),
                    new TestSyncOperation()
                };

                SyncService sut = new SyncServiceFixture().WithOperations(operations);

                // When
                var result = sut.GetAll();

                // Then
                result.Should().Contain(operations);
            }
        }

        public class TheGetInitialMethod
        {
            [Fact]
            public void Should_Return_Initial()
            {
                // Given
                var operations = new List<ISyncOperation>
                {
                    new TestSyncOperation(),
                    new TestSyncOperation{ Initial = false },
                    new TestSyncOperation()
                };

                SyncService sut = new SyncServiceFixture().WithOperations(operations);

                // When
                var result = sut.GetInitial().ToList();

                // Then
                result.Should().NotContain(x => x.Initial == false);
                result.Count.Should().Be(2);
            }

            [Fact]
            public void Should_Not_Return_All()
            {
                // Given
                var operations = new List<ISyncOperation>
                {
                    new TestSyncOperation(),
                    new TestSyncOperation{ Initial = false },
                    new TestSyncOperation{ Initial = false }
                };

                SyncService sut = new SyncServiceFixture().WithOperations(operations);

                // When
                var result = sut.GetInitial().ToList();

                // Then
                result.Should().NotContain(x => x.Initial == false);
                result.Count.Should().Be(1);
            }
        }
    }
}
