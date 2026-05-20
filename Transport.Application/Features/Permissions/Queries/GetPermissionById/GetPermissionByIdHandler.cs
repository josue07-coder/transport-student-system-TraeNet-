using MediatR;
using Transport.Application.Features.Permissions.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Permissions.Queries.GetPermissionById
{
    public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, PermissionResponseDto>
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionByIdHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<PermissionResponseDto> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Permiso no encontrado");

            return PermissionMapper.ToResponseDto(permission);
        }
    }
}
