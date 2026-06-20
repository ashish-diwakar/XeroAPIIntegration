namespace XeroDemo.Interfaces
{
    public interface IXeroTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
