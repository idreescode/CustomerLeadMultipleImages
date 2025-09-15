using CustomerLeadApi.Models;

namespace CustomerLeadApi.Interfaces
{
    public interface IContactImageRepository : IRepository<ContactImage>
    {
        Task<IEnumerable<ContactImage>> GetImagesByContactIdAsync(int contactId);
        Task<ContactImage?> GetImageByContactAndImageIdAsync(int contactId, int imageId);
        Task<bool> DeleteImageByContactAndImageIdAsync(int contactId, int imageId);
    }
}
