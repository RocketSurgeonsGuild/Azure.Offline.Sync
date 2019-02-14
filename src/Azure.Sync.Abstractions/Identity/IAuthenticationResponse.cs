namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface IAuthenticationResponse
    {
        IApiContext ApiSession { get; set; }
        IUserSession UserSession { get; set; }
        IJwt Token { get; set; }
    }
}