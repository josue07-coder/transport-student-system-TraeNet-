using FluentValidation;
using Transport.Application.Features.TripRouteDeviations.Commands.ReportTripRouteDeviation;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripRouteDeviations.Validators
{
    public class ReportTripRouteDeviationValidator : AbstractValidator<ReportTripRouteDeviationCommand>
    {
        public ReportTripRouteDeviationValidator()
        {
            RuleFor(x => x.TripId).NotEmpty();

            RuleFor(x => x.ReasonType)
                .IsInEnum();

            RuleFor(x => x.Reason)
                .MaximumLength(300)
                .NotEmpty()
                .When(x => x.ReasonType == RouteDeviationReasonType.Other);

            RuleFor(x => x.Notes)
                .MaximumLength(1000);

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90m, 90m)
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180m, 180m)
                .When(x => x.Longitude.HasValue);
        }
    }
}
