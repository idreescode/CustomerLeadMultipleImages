using CustomerLeadApi.Common;
using CustomerLeadApi.DTOs;
using CustomerLeadApi.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CustomerLeadApi.Controllers
{
    [Route("api/contacts/{contactId}/images")]
    [ApiController]
    public class ContactImagesController : ControllerBase
    {
        private readonly IContactImageService _imageService;
        private readonly IValidator<UploadImageDTO> _uploadValidator;
        private readonly ILogger<ContactImagesController> _logger;

        public ContactImagesController(
            IContactImageService imageService,
            IValidator<UploadImageDTO> uploadValidator,
            ILogger<ContactImagesController> logger)
        {
            _imageService = imageService;
            _uploadValidator = uploadValidator;
            _logger = logger;
        }

        /// <summary>
        /// Get all images for a contact
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ContactImageDTO>>>> GetContactImages(int contactId)
        {
            var result = await _imageService.GetContactImagesAsync(contactId);
            return Ok(result);
        }

        /// <summary>
        /// Get specific image for a contact
        /// </summary>
        [HttpGet("{imageId}")]
        public async Task<ActionResult<ApiResponse<ContactImageDTO>>> GetContactImage(int contactId, int imageId)
        {
            var result = await _imageService.GetContactImageAsync(contactId, imageId);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Upload a single image for a contact
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ContactImageDTO>>> UploadImage(int contactId, UploadImageDTO uploadImageDTO)
        {
            var validationResult = await _uploadValidator.ValidateAsync(uploadImageDTO);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ContactImageDTO>.ErrorResult("Validation failed", errors));
            }

            var result = await _imageService.UploadImageAsync(contactId, uploadImageDTO);
            
            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                if (result.Message.Contains("Maximum number of images"))
                    return BadRequest(result);
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetContactImage), new { contactId, imageId = result.Data!.Id }, result);
        }

        /// <summary>
        /// Upload multiple images for a contact
        /// </summary>
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ContactImageDTO>>>> UploadMultipleImages(int contactId, List<UploadImageDTO> uploadImageDTOs)
        {
            if (uploadImageDTOs == null || !uploadImageDTOs.Any())
            {
                return BadRequest(ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult("No images provided"));
            }

            // Validate all images
            var validationErrors = new List<string>();
            foreach (var imageDto in uploadImageDTOs)
            {
                var validationResult = await _uploadValidator.ValidateAsync(imageDto);
                if (!validationResult.IsValid)
                {
                    validationErrors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
                }
            }

            if (validationErrors.Any())
            {
                return BadRequest(ApiResponse<IEnumerable<ContactImageDTO>>.ErrorResult("Validation failed", validationErrors));
            }

            var result = await _imageService.UploadMultipleImagesAsync(contactId, uploadImageDTOs);
            
            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                if (result.Message.Contains("Cannot upload"))
                    return BadRequest(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete an image from a contact
        /// </summary>
        [HttpDelete("{imageId}")]
        public async Task<ActionResult<ApiResponse>> DeleteImage(int contactId, int imageId)
        {
            var result = await _imageService.DeleteImageAsync(contactId, imageId);
            
            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Validate if contact can add more images
        /// </summary>
        [HttpGet("validate-limit")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateImageLimit(int contactId, [FromQuery] int additionalImages = 1)
        {
            var result = await _imageService.ValidateImageLimitAsync(contactId, additionalImages);
            return Ok(result);
        }
    }
}
