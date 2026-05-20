using Transport.Domain.Entities;

namespace Transport.Application.Features.Roles.DTOs
{
    internal static class RoleMapper
    {
        public static RoleResponseDto ToResponseDto(Role role)
        {
            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        public static RoleDetailDto ToDetailDto(Role role)
        {
            return new RoleDetailDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.RolePermissions
                    .Select(rolePermission => rolePermission.Permission.Name)
                    .OrderBy(permission => permission)
                    .ToList()
            };
        }
    }
}
