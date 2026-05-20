using FluentValidation;
using Transport.Application.Features.Trips.Commands.StartTrip;

namespace Transport.Application.Features.Trips.Validators
{
    public class StartTripValidator : AbstractValidator<StartTripCommand>
    {
        public StartTripValidator()
        {
            RuleFor(x => x.RouteAssignmentId).NotEmpty();
        }
    }
}
