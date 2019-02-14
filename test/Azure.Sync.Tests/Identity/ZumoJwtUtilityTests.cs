using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Rocket.Surgery.Azure.Sync;
using Rocket.Surgery.Azure.Sync.Abstractions;
using Xunit;

namespace Azure.Sync.Tests
{
    public sealed class ZumoJwtUtilityTests
    {
        private const string JwtString =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqZG9lIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJPcmdhbml6YXRpb24iOiIxMjM0NTYiLCJGYWNpbGl0aWVzIjpbIjEiLCIyIiwiMyIsIjQiXSwiUm9sZSI6IkFkbWluIiwiYXVkIjoiQWRtaW5pc3RyYXRvcnMiLCJleHAiOjE1MDAwLCJpc3MiOiJodHRwczovL2lzc3Vlci5jb20iLCJ2ZXIiOiIxLjIuMS45MDAifQ.PKDW7xpRR3oVRiQVKl3supFSIgaRRo1y2NjW-zx5V2A";

        private static string Payload = JwtString.Split('.').Skip(1).First();

        public class TheDecodePayloadMethod
        {
            [Fact]
            public void Should_Throw_If_Payload_Not_Valid()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = Record.Exception(() =>  sut.DecodePayload("00001"));

                // Then
                result.Should().NotBeNull();
                result.Should().BeOfType<InvalidTokenException>();
            }

            [Fact]
            public void Should_Throw_If_Payload_Not_Formatted()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = Record.Exception(() => sut.DecodePayload(JwtString));

                // Then
                result.Should().NotBeNull();
                result.Should().BeOfType<FormatException>();
            }

            [Fact]
            public void Should_Decode_Payload()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.DecodePayload(Payload);

                // Then
                result.Should().Be("{\"sub\":\"jdoe\",\"name\":\"John Doe\",\"iat\":1516239022,\"Organization\":\"123456\",\"Facilities\":[\"1\",\"2\",\"3\",\"4\"],\"Role\":\"Admin\",\"aud\":\"Administrators\",\"exp\":15000,\"iss\":\"https://issuer.com\",\"ver\":\"1.2.1.900\"}");
            }
        }

        public class TheGetTokenMethod
        {
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void Should_Throw_If_Jwt_Not_Valid(int take)
            {
                // Given
                var invalidToken = string.Join('.', JwtString.Split('.').Take(take));
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = Record.Exception(() => sut.GetToken<TestJwt>(invalidToken));

                // Then
                result.Should().BeOfType<InvalidTokenException>();
            }

            [Fact]
            public void Should_Return_Organization()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Organization.Should().Be("123456");
            }

            [Fact]
            public void Should_Return_Role()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Role.Should().Be("Admin");
            }

            [Fact]
            public void Should_Return_Issuer()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Issuer.Should().Be("https://issuer.com");
            }

            [Fact]
            public void Should_Return_Subject()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Subject.Should().Be("jdoe");
            }

            [Fact]
            public void Should_Return_Version()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Version.Should().Be("1.2.1.900");
            }

            [Fact]
            public void Should_Return_Facilities()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Facilities.Should().Contain(x => x == "1");
                result.Facilities.Should().Contain(x => x == "2");
                result.Facilities.Should().Contain(x => x == "3");
                result.Facilities.Should().Contain(x => x == "4");
            }

            [Fact]
            public void Should_Return_Token_Of_Type()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetToken<TestJwt>(JwtString);

                // Then
                result.Should().BeOfType<TestJwt>();
            }
        }

        public class TheGetExpirationMethod
        {
            [Fact]
            public void Should_Return_Expiration()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetTokenExpiration<TestJwt>(JwtString);

                // Then
                result.Should().NotBeNull();
            }

            [Fact]
            public void Should_Return_Expiration_Time()
            {
                // Given
                ZumoJwtUtility sut = new ZumoJwtUtility();

                // When
                var result = sut.GetTokenExpiration<TestJwt>(JwtString);

                // Then
                result.Should().Be(Convert.ToDateTime("1/1/1970 4:10:00 AM"));
            }
        }
    }
}