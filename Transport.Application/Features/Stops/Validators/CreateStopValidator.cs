using FluentValidation;
using Transport.Application.Features.Stops.Commands.CreateStop;

namespace Transport.Application.Features.Stops.Validators
{
    public class CreateStopValidator : AbstractValidator<CreateStopCommand>
    {
        public CreateStopValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
            RuleFor(x => x.SectorId).NotEmpty();
        }
    }
}
