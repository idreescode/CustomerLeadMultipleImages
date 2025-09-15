using CustomerLeadApi.DTOs;
using FluentValidation;

namespace CustomerLeadApi.Validators
{
    public class UploadImageValidator : AbstractValidator<UploadImageDTO>
    {
        public UploadImageValidator()
        {
            RuleFor(x => x.ImageData)
                .NotEmpty().WithMessage("Image data is required")
                .Must(BeValidBase64).WithMessage("Invalid base64 image data");

            RuleFor(x => x.FileName)
                .MaximumLength(100).WithMessage("File name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FileName));

            RuleFor(x => x.ContentType)
                .Must(BeValidImageType).WithMessage("Invalid image type")
                .When(x => !string.IsNullOrEmpty(x.ContentType));
        }

        private bool BeValidBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool BeValidImageType(string? contentType)
        {
            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return validTypes.Contains(contentType?.ToLower());
        }
    }
}
