using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;

namespace CustomerLeadApi.Interfaces
{
    public interface IContactService
    {
        Task<ApiResponse<IEnumerable<ContactDTO>>> GetAllContactsAsync();
        Task<ApiResponse<ContactDTO>> GetContactByIdAsync(int id);
        Task<ApiResponse<ContactDTO>> CreateContactAsync(CreateContactDTO createContactDTO);
        Task<ApiResponse<ContactDTO>> UpdateContactAsync(int id, UpdateContactDTO updateContactDTO);
        Task<ApiResponse> DeleteContactAsync(int id);
        Task<ApiResponse<bool>> CanAddMoreImagesAsync(int contactId, int additionalImages = 1);
    }
}
