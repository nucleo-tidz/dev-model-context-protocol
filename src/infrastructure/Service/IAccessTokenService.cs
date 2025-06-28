namespace infrastructure.Service
{
    public interface IAccessTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
