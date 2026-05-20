using FluentValidation;
using Transport.Application.Features.Roles.Commands.AssignPermissionToRole;

namespace Transport.Application.Features.Roles.Validators
{
    public class AssignPermissionToRoleValidator : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.PermissionId).NotEmpty();
        }
    }
}
