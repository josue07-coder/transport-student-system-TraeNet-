using FluentValidation;
using Transport.Application.Features.Routes.Commands.AddStopToRoute;

namespace Transport.Application.Features.Routes.Validators
{
    public class AddStopToRouteValidator : AbstractValidator<AddStopToRouteCommand>
    {
        public AddStopToRouteValidator()
        {
            RuleFor(x => x.RouteId).NotEmpty();
            RuleFor(x => x.StopId).NotEmpty();
            RuleFor(x => x.StopOrder).GreaterThan(0);
        }
    }
}
