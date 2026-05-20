using FluentValidation;
using Transport.Application.Features.Drivers.Commands.CreateDriver;

namespace Transport.Application.Features.Drivers.Validators
{
    public class CreateDriverValidator : AbstractValidator<CreateDriverCommand>
    {
        public CreateDriverValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DocumentType).IsInEnum();
            RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.PhotoUrl).MaximumLength(300);
        }
    }
}
