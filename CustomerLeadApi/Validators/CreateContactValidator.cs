using CustomerLeadApi.DTOs;
using FluentValidation;

namespace CustomerLeadApi.Validators
{
    public class CreateContactValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters")
                .Matches(@"^[\+]?[1-9][\d]{0,15}$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid contact type");
        }
    }
}
