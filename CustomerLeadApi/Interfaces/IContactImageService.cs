using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;

namespace CustomerLeadApi.Interfaces
{
    public interface IContactImageService
    {
        Task<ApiResponse<IEnumerable<ContactImageDTO>>> GetContactImagesAsync(int contactId);
        Task<ApiResponse<ContactImageDTO>> GetContactImageAsync(int contactId, int imageId);
        Task<ApiResponse<ContactImageDTO>> UploadImageAsync(int contactId, UploadImageDTO uploadImageDTO);
        Task<ApiResponse<IEnumerable<ContactImageDTO>>> UploadMultipleImagesAsync(int contactId, List<UploadImageDTO> uploadImageDTOs);
        Task<ApiResponse> DeleteImageAsync(int contactId, int imageId);
        Task<ApiResponse<bool>> ValidateImageLimitAsync(int contactId, int additionalImages = 1);
    }
}
