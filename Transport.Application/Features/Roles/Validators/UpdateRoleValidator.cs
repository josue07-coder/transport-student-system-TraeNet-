using FluentValidation;
using Transport.Application.Features.Roles.Commands.UpdateRole;

namespace Transport.Application.Features.Roles.Validators
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(300);
        }
    }
}
