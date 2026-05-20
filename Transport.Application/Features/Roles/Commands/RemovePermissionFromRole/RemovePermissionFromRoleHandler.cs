using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand, Unit>
    {
        private readonly IRoleRepository _roleRepository;

        public RemovePermissionFromRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            role.RemovePermission(request.PermissionId);
            await _roleRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
