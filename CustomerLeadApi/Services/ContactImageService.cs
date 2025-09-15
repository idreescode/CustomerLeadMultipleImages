using AutoMapper;
using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;
using CustomerLeadApi.Interfaces;
using CustomerLeadApi.Models;

namespace CustomerLeadApi.Services
{
    public class ContactImageService : IContactImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactImageService> _logger;
        private const int MaxImagesPerContact = 10;

        public ContactImageService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContactImageService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ContactImageDTO>>> GetContactImagesAsync(int contactId)
        {
            try
            {
                _logger.LogInformation("Getting images for contact ID: {ContactId}", contactId);
                
                var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {ContactId} not found", contactId);
                    return ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult($"Contact with ID {contactId} not found");
                }

                var images = await _unitOfWork.ContactImages.GetImagesByContactIdAsync(contactId);
                var imageDTOs = _mapper.Map<IEnumerable<ContactImageDTO>>(images);
                
                return ApiResponse<IEnumerable<ContactImageDTO>>.SuccessResult(imageDTOs, "Images retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting images for contact ID: {ContactId}", contactId);
                return ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult("Failed to retrieve images", ex.Message);
            }
        }

        public async Task<ApiResponse<ContactImageDTO>> GetContactImageAsync(int contactId, int imageId)
        {
            try
            {
                _logger.LogInformation("Getting image {ImageId} for contact ID: {ContactId}", imageId, contactId);
                
                var image = await _unitOfWork.ContactImages.GetImageByContactAndImageIdAsync(contactId, imageId);
                if (image == null)
                {
                    _logger.LogWarning("Image {ImageId} not found for contact ID: {ContactId}", imageId, contactId);
                    return ApiResponse<ContactImageDTO>.ErrorResult($"Image with ID {imageId} not found for contact {contactId}");
                }

                var imageDTO = _mapper.Map<ContactImageDTO>(image);
                return ApiResponse<ContactImageDTO>.SuccessResult(imageDTO, "Image retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting image {ImageId} for contact ID: {ContactId}", imageId, contactId);
                return ApiResponse<ContactImageDTO>.ErrorResult("Failed to retrieve image", ex.Message);
            }
        }

        public async Task<ApiResponse<ContactImageDTO>> UploadImageAsync(int contactId, UploadImageDTO uploadImageDTO)
        {
            try
            {
                _logger.LogInformation("Uploading image for contact ID: {ContactId}", contactId);
                
                // Validate image limit
                var canAdd = await ValidateImageLimitAsync(contactId, 1);
                if (!canAdd.Data)
                {
                    return ApiResponse<ContactImageDTO>.ErrorResult($"Maximum number of images ({MaxImagesPerContact}) reached for this contact");
                }

                var image = _mapper.Map<ContactImage>(uploadImageDTO);
                image.ContactId = contactId;
                
                await _unitOfWork.ContactImages.AddAsync(image);
                await _unitOfWork.SaveChangesAsync();

                var imageDTO = _mapper.Map<ContactImageDTO>(image);
                _logger.LogInformation("Image uploaded successfully with ID: {ImageId} for contact ID: {ContactId}", image.Id, contactId);
                
                return ApiResponse<ContactImageDTO>.SuccessResult(imageDTO, "Image uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading image for contact ID: {ContactId}", contactId);
                return ApiResponse<ContactImageDTO>.ErrorResult("Failed to upload image", ex.Message);
            }
        }

        public async Task<ApiResponse<IEnumerable<ContactImageDTO>>> UploadMultipleImagesAsync(int contactId, List<UploadImageDTO> uploadImageDTOs)
        {
            try
            {
                _logger.LogInformation("Uploading {ImageCount} images for contact ID: {ContactId}", uploadImageDTOs.Count, contactId);
                
                // Validate image limit
                var canAdd = await ValidateImageLimitAsync(contactId, uploadImageDTOs.Count);
                if (!canAdd.Data)
                {
                    var currentCount = await _unitOfWork.Contacts.GetImageCountAsync(contactId);
                    var maxAllowed = MaxImagesPerContact - currentCount;
                    return ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult($"Cannot upload {uploadImageDTOs.Count} images. Maximum allowed is {maxAllowed} more images");
                }

                var images = _mapper.Map<List<ContactImage>>(uploadImageDTOs);
                foreach (var image in images)
                {
                    image.ContactId = contactId;
                }

                await _unitOfWork.ContactImages.AddRangeAsync(images.AsEnumerable());
                await _unitOfWork.SaveChangesAsync();

                var imageDTOs = _mapper.Map<IEnumerable<ContactImageDTO>>(images);
                _logger.LogInformation("Multiple images uploaded successfully for contact ID: {ContactId}", contactId);
                
                return ApiResponse<IEnumerable<ContactImageDTO>>.SuccessResult(imageDTOs, "Images uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading multiple images for contact ID: {ContactId}", contactId);
                return ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult("Failed to upload images", ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteImageAsync(int contactId, int imageId)
        {
            try
            {
                _logger.LogInformation("Deleting image {ImageId} for contact ID: {ContactId}", imageId, contactId);
                
                var deleted = await _unitOfWork.ContactImages.DeleteImageByContactAndImageIdAsync(contactId, imageId);
                if (!deleted)
                {
                    _logger.LogWarning("Image {ImageId} not found for contact ID: {ContactId}", imageId, contactId);
                    return ApiResponse.ErrorResult($"Image with ID {imageId} not found for contact {contactId}");
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Image deleted successfully with ID: {ImageId} for contact ID: {ContactId}", imageId, contactId);
                
                return ApiResponse.SuccessResult("Image deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting image {ImageId} for contact ID: {ContactId}", imageId, contactId);
                return ApiResponse.ErrorResult("Failed to delete image", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> ValidateImageLimitAsync(int contactId, int additionalImages = 1)
        {
            try
            {
                var canAdd = await _unitOfWork.Contacts.CanAddMoreImagesAsync(contactId, additionalImages);
                return ApiResponse<bool>.SuccessResult(canAdd, canAdd ? "Can add more images" : "Image limit reached");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating image limit for contact ID: {ContactId}", contactId);
                return ApiResponse<bool>.ErrorResult("Failed to validate image limit", ex.Message);
            }
        }
    }
}
