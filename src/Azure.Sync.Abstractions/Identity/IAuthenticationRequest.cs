namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface IAuthenticationRequest
    {
        string Password { get; set; }
        string UserName { get; set; }
    }
}