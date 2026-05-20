using MediatR;
using Transport.Application.Features.Roles.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, RoleResponseDto>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleResponseDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Rol no encontrado");

            role.Update(request.Name, request.Description);
            await _roleRepository.SaveChangesAsync();

            return RoleMapper.ToResponseDto(role);
        }
    }
}
