using Transport.Domain.Entities;

namespace Transport.Application.Features.Users.DTOs
{
    internal static class UserMapper
    {
        public static UserResponseDto ToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                IsActive = user.IsActive,
                ProfileImageUrl = user.ProfileImageUrl,
                GuardianId = user.GuardianId,
                DriverId = user.DriverId,
                TransportAssistantId = user.TransportAssistantId
            };
        }

        public static UserDetailDto ToDetailDto(User user)
        {
            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                Role = user.Role.Name,
                Permissions = user.Role.RolePermissions
                    .Select(rolePermission => rolePermission.Permission.Name)
                    .OrderBy(permission => permission)
                    .ToList(),
                IsActive = user.IsActive,
                ProfileImageUrl = user.ProfileImageUrl,
                GuardianId = user.GuardianId,
                DriverId = user.DriverId,
                TransportAssistantId = user.TransportAssistantId,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
