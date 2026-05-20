using FluentValidation;
using Transport.Application.Features.Routes.Commands.CreateRoute;

namespace Transport.Application.Features.Routes.Validators
{
    public class CreateRouteValidator : AbstractValidator<CreateRouteCommand>
    {
        public CreateRouteValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SchoolId).NotEmpty();
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        }
    }
}
