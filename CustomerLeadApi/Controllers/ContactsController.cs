using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;
using CustomerLeadApi.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CustomerLeadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IValidator<CreateContactDTO> _createValidator;
        private readonly IValidator<UpdateContactDTO> _updateValidator;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(
            IContactService contactService,
            IValidator<CreateContactDTO> createValidator,
            IValidator<UpdateContactDTO> updateValidator,
            ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Get all contacts with image count
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ContactDTO>>>> GetContacts()
        {
            var result = await _contactService.GetAllContactsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get contact by ID with images
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ContactDTO>>> GetContact(int id)
        {
            var result = await _contactService.GetContactByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ContactDTO>>> CreateContact(CreateContactDTO createContactDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(createContactDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ContactDTO>.ErrorResult("Validation failed", errors));
            }

            var result = await _contactService.CreateContactAsync(createContactDTO);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetContact), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Update an existing contact
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ContactDTO>>> UpdateContact(int id, UpdateContactDTO updateContactDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateContactDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ContactDTO>.ErrorResult("Validation failed", errors));
            }

            var result = await _contactService.UpdateContactAsync(id, updateContactDTO);
            
            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteContact(int id)
        {
            var result = await _contactService.DeleteContactAsync(id);
            
            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Check if contact can add more images
        /// </summary>
        [HttpGet("{id}/can-add-images")]
        public async Task<ActionResult<ApiResponse<bool>>> CanAddImages(int id, [FromQuery] int additionalImages = 1)
        {
            var result = await _contactService.CanAddMoreImagesAsync(id, additionalImages);
            return Ok(result);
        }
    }
}
