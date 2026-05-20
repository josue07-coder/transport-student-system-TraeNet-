using FluentValidation;
using Transport.Application.Features.Routes.Commands.UpdateRouteStopOrder;

namespace Transport.Application.Features.Routes.Validators
{
    public class UpdateRouteStopOrderValidator : AbstractValidator<UpdateRouteStopOrderCommand>
    {
        public UpdateRouteStopOrderValidator()
        {
            RuleFor(x => x.RouteId).NotEmpty();
            RuleFor(x => x.StopId).NotEmpty();
            RuleFor(x => x.StopOrder).GreaterThan(0);
        }
    }
}
