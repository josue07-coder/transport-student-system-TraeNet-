using MediatR;
using Transport.Application.Features.Permissions.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Permissions.Queries.GetAllPermissions
{
    public class GetAllPermissionsHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionResponseDto>>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<PermissionResponseDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return permissions.Select(PermissionMapper.ToResponseDto).ToList();
        }
    }
}
