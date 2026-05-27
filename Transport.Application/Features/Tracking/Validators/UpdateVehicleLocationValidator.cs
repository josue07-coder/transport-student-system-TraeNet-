using FluentValidation;
using Transport.Application.Features.Tracking.Commands.UpdateVehicleLocation;

namespace Transport.Application.Features.Tracking.Validators
{
    public class UpdateVehicleLocationValidator : AbstractValidator<UpdateVehicleLocationCommand>
    {
        public UpdateVehicleLocationValidator()
        {
            RuleFor(command => command.TripId).NotEmpty();
            RuleFor(command => command.Latitude).InclusiveBetween(-90, 90);
            RuleFor(command => command.Longitude).InclusiveBetween(-180, 180);
            RuleFor(command => command.Speed).GreaterThanOrEqualTo(0).When(command => command.Speed.HasValue);
            RuleFor(command => command.Heading).InclusiveBetween(0, 360).When(command => command.Heading.HasValue);
        }
    }
}
