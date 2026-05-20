using FluentValidation;
using Transport.Application.Features.TransportAssistants.Commands.UpdateTransportAssistant;

namespace Transport.Application.Features.TransportAssistants.Validators
{
    public class UpdateTransportAssistantValidator : AbstractValidator<UpdateTransportAssistantCommand>
    {
        public UpdateTransportAssistantValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DocumentType).IsInEnum();
            RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).EmailAddress().MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.PhotoUrl).MaximumLength(300);
        }
    }
}
