using AutoMapper;
using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;
using CustomerLeadApi.Interfaces;
using CustomerLeadApi.Models;

namespace CustomerLeadApi.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContactService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ContactDTO>>> GetAllContactsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all contacts");
                var contacts = await _unitOfWork.Contacts.GetContactsWithImageCountAsync();
                var contactDTOs = _mapper.Map<IEnumerable<ContactDTO>>(contacts);
                
                return ApiResponse<IEnumerable<ContactDTO>>.SuccessResult(contactDTOs, "Contacts retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all contacts");
                return ApiResponse<IEnumerable<ContactDTO>>.ErrorResult("Failed to retrieve contacts", ex.Message);
            }
        }

        public async Task<ApiResponse<ContactDTO>> GetContactByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting contact with ID: {ContactId}", id);
                var contact = await _unitOfWork.Contacts.GetContactWithImagesAsync(id);
                
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {ContactId} not found", id);
                    return ApiResponse<ContactDTO>.ErrorResult($"Contact with ID {id} not found");
                }

                var contactDTO = _mapper.Map<ContactDTO>(contact);
                return ApiResponse<ContactDTO>.SuccessResult(contactDTO, "Contact retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting contact with ID: {ContactId}", id);
                return ApiResponse<ContactDTO>.ErrorResult("Failed to retrieve contact", ex.Message);
            }
        }

        public async Task<ApiResponse<ContactDTO>> CreateContactAsync(CreateContactDTO createContactDTO)
        {
            try
            {
                _logger.LogInformation("Creating new contact: {ContactName}", createContactDTO.Name);
                
                var contact = _mapper.Map<Contact>(createContactDTO);
                await _unitOfWork.Contacts.AddAsync(contact);
                await _unitOfWork.SaveChangesAsync();

                var contactDTO = _mapper.Map<ContactDTO>(contact);
                _logger.LogInformation("Contact created successfully with ID: {ContactId}", contact.Id);
                
                return ApiResponse<ContactDTO>.SuccessResult(contactDTO, "Contact created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating contact: {ContactName}", createContactDTO.Name);
                return ApiResponse<ContactDTO>.ErrorResult("Failed to create contact", ex.Message);
            }
        }

        public async Task<ApiResponse<ContactDTO>> UpdateContactAsync(int id, UpdateContactDTO updateContactDTO)
        {
            try
            {
                _logger.LogInformation("Updating contact with ID: {ContactId}", id);
                
                var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {ContactId} not found for update", id);
                    return ApiResponse<ContactDTO>.ErrorResult($"Contact with ID {id} not found");
                }

                _mapper.Map(updateContactDTO, contact);
                await _unitOfWork.Contacts.UpdateAsync(contact);
                await _unitOfWork.SaveChangesAsync();

                var contactDTO = _mapper.Map<ContactDTO>(contact);
                _logger.LogInformation("Contact updated successfully with ID: {ContactId}", id);
                
                return ApiResponse<ContactDTO>.SuccessResult(contactDTO, "Contact updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact with ID: {ContactId}", id);
                return ApiResponse<ContactDTO>.ErrorResult("Failed to update contact", ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteContactAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting contact with ID: {ContactId}", id);
                
                var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {ContactId} not found for deletion", id);
                    return ApiResponse.ErrorResult($"Contact with ID {id} not found");
                }

                await _unitOfWork.Contacts.DeleteAsync(contact);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Contact deleted successfully with ID: {ContactId}", id);
                return ApiResponse.SuccessResult("Contact deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting contact with ID: {ContactId}", id);
                return ApiResponse.ErrorResult("Failed to delete contact", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> CanAddMoreImagesAsync(int contactId, int additionalImages = 1)
        {
            try
            {
                var canAdd = await _unitOfWork.Contacts.CanAddMoreImagesAsync(contactId, additionalImages);
                return ApiResponse<bool>.SuccessResult(canAdd, canAdd ? "Can add more images" : "Image limit reached");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking image limit for contact ID: {ContactId}", contactId);
                return ApiResponse<bool>.ErrorResult("Failed to check image limit", ex.Message);
            }
        }
    }
}
