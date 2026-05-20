using Transport.Domain.Entities;

namespace Transport.Application.Features.Permissions.DTOs
{
    internal static class PermissionMapper
    {
        public static PermissionResponseDto ToResponseDto(Permission permission)
        {
            return new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Module = permission.Module
            };
        }
    }
}
