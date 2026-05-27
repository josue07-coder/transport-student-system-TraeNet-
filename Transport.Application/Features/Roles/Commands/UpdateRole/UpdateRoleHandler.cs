using MediatR;
using Transport.Application.Features.Roles.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, RoleResponseDto>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditService _auditService;

        public UpdateRoleHandler(IRoleRepository roleRepository, IAuditService auditService)
        {
            _roleRepository = roleRepository;
            _auditService = auditService;
        }

        public async Task<RoleResponseDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Rol no encontrado");

            var oldValues = $"{{\"Name\":\"{role.Name}\",\"Description\":\"{role.Description}\"}}";
            role.Update(request.Name, request.Description);
            await _roleRepository.SaveChangesAsync();
            var newValues = $"{{\"Name\":\"{role.Name}\",\"Description\":\"{role.Description}\"}}";
            await _auditService.LogAsync("Updated", "Role", role.Id.ToString(), oldValues, newValues);

            return RoleMapper.ToResponseDto(role);
        }
    }
}
