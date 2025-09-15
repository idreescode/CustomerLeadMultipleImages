using CustomerLeadApi.Data;
using CustomerLeadApi.Interfaces;
using CustomerLeadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerLeadApi.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Contact?> GetContactWithImagesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetContactsWithImageCountAsync()
        {
            return await _dbSet
                .Include(c => c.Images)
                .ToListAsync();
        }

        public async Task<bool> CanAddMoreImagesAsync(int contactId, int additionalImages = 1)
        {
            var imageCount = await _context.ContactImages
                .CountAsync(ci => ci.ContactId == contactId);
            
            return (imageCount + additionalImages) <= 10;
        }

        public async Task<int> GetImageCountAsync(int contactId)
        {
            return await _context.ContactImages
                .CountAsync(ci => ci.ContactId == contactId);
        }
    }
}
