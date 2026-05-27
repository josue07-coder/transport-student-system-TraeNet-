using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand, Unit>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditService _auditService;

        public RemovePermissionFromRoleHandler(IRoleRepository roleRepository, IAuditService auditService)
        {
            _roleRepository = roleRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(request.RoleId)
                ?? throw new DomainException("Rol no encontrado");

            role.RemovePermission(request.PermissionId);
            await _roleRepository.SaveChangesAsync();

            await _auditService.LogAsync("Removed", "RolePermission", role.Id.ToString(), $"{{\"PermissionId\":\"{request.PermissionId}\"}}");

            return Unit.Value;
        }
    }
}
