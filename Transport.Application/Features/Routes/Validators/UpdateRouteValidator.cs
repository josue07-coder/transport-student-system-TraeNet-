using FluentValidation;
using Transport.Application.Features.Routes.Commands.UpdateRoute;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Routes.Validators
{
    public class UpdateRouteValidator : AbstractValidator<UpdateRouteCommand>
    {
        public UpdateRouteValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SchoolId).NotEmpty();
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
            RuleFor(x => x.Status)
                .Must(status => status == RouteStatus.Active || status == RouteStatus.Inactive)
                .WithMessage("Solo se permite actualizar la ruta a Active o Inactive.");
        }
    }
}
