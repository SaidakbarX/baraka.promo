namespace baraka.promo.Services
{
    public interface ICurrentUser
    {
        string GetCurrentUserName();
        bool IsAuthenticated();
        bool IsAdmin();
        Guid GetCurrentClientId();
        string GetCurrentToken();
        bool IsPOS();
    }
}
