using System.Linq;
using FluentAssertions;
using Rocket.Surgery.Azure;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class SessionSyncServiceTests
    {
        public class TheGetSessionMethod
        {
            [Fact]
            public void Should_Return_Session()
            {
                // Given
                SessionSyncService sut = new SessionSyncServiceFixture();

                // When
                var result = sut.GetSession().ToList();

                // Then
                result.Count.Should().Be(1);
            }
        }
    }
}