using MediatR;
using Transport.Application.Features.Roles.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, List<RoleResponseDto>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleResponseDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(RoleMapper.ToResponseDto).ToList();
        }
    }
}
