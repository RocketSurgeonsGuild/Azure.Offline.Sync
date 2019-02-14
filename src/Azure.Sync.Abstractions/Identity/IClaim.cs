using System.Collections;

namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface IClaim
    {
        string Issuer { get; }

        string Type { get; }

        string Value { get; }
    }
}