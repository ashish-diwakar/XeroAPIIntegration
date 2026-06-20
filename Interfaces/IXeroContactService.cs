using XeroDemo.Models;

namespace XeroDemo.Interfaces
{
    public interface IXeroContactService
    {
        Task<List<XeroContactDto>> GetContactsAsync();
    }
}
