using System;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public class InvalidTokenException : Exception
    {
        /// <inheritdoc />
        public InvalidTokenException() { }

        /// <inheritdoc />
        public InvalidTokenException(string message) : base(message) { }

        /// <inheritdoc />
        public InvalidTokenException(string message, Exception inner) : base(message, inner) { }
    }
}