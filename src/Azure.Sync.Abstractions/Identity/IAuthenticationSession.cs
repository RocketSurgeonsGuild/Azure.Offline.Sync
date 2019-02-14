namespace Rocket.Surgery.Azure.Sync.Abstractions
{
    public interface IAuthenticationSession
    {
        IApiContext ApiContext { get; set; }
        IUserSession UserSession { get; set; }
    }
}