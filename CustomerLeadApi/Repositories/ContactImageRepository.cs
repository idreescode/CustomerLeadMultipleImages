using CustomerLeadApi.Data;
using CustomerLeadApi.Interfaces;
using CustomerLeadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerLeadApi.Repositories
{
    public class ContactImageRepository : Repository<ContactImage>, IContactImageRepository
    {
        public ContactImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ContactImage>> GetImagesByContactIdAsync(int contactId)
        {
            return await _dbSet
                .Where(ci => ci.ContactId == contactId)
                .OrderBy(ci => ci.UploadedAt)
                .ToListAsync();
        }

        public async Task<ContactImage?> GetImageByContactAndImageIdAsync(int contactId, int imageId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ci => ci.ContactId == contactId && ci.Id == imageId);
        }

        public async Task<bool> DeleteImageByContactAndImageIdAsync(int contactId, int imageId)
        {
            var image = await GetImageByContactAndImageIdAsync(contactId, imageId);
            if (image == null)
                return false;

            _dbSet.Remove(image);
            return true;
        }
    }
}
