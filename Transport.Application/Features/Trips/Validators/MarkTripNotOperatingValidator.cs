using FluentValidation;
using Transport.Application.Features.Trips.Commands.MarkTripNotOperating;

namespace Transport.Application.Features.Trips.Validators
{
    public class MarkTripNotOperatingValidator : AbstractValidator<MarkTripNotOperatingCommand>
    {
        public MarkTripNotOperatingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }
}
