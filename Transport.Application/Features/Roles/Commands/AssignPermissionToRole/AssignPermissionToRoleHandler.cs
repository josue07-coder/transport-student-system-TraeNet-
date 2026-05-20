using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Roles.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleHandler : IRequestHandler<AssignPermissionToRoleCommand, Unit>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public AssignPermissionToRoleHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<Unit> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            if (!await _permissionRepository.ExistsAsync(request.PermissionId))
                throw new DomainException("Permiso no encontrado");

            role.AssignPermission(request.PermissionId);
            await _roleRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
