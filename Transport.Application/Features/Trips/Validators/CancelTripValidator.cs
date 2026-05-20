using FluentValidation;
using Transport.Application.Features.Trips.Commands.CancelTrip;

namespace Transport.Application.Features.Trips.Validators
{
    public class CancelTripValidator : AbstractValidator<CancelTripCommand>
    {
        public CancelTripValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
