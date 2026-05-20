using FluentValidation;
using Transport.Application.Features.Trips.Commands.EndTrip;

namespace Transport.Application.Features.Trips.Validators
{
    public class EndTripValidator : AbstractValidator<EndTripCommand>
    {
        public EndTripValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
