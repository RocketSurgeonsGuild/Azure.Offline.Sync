using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Rocket.Surgery.Azure.Sync.Handlers;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class HttpRequestMessageExtensionTests
    {
        public class WithVersionMethod
        {
            [Fact]
            public void Should_Return_Accept_Header()
            {
                // Given
                var message = new HttpRequestMessage();

                // When
                message.WithVersion(1);

                // Then
                message.Headers.GetValues("Accept").First().Should().Be("application/json; ver=1");
            }
        }
    }
}
