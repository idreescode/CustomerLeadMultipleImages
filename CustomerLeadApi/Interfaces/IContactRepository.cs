using CustomerLeadApi.Models;

namespace CustomerLeadApi.Interfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<Contact?> GetContactWithImagesAsync(int id);
        Task<IEnumerable<Contact>> GetContactsWithImageCountAsync();
        Task<bool> CanAddMoreImagesAsync(int contactId, int additionalImages = 1);
        Task<int> GetImageCountAsync(int contactId);
    }
}
