using Transport.Domain.Entities;

namespace Transport.Application.Features.SystemSettings.DTOs
{
    internal static class SystemSettingMappings
    {
        public static SystemSettingResponseDto ToResponseDto(SystemSetting setting)
        {
            return new SystemSettingResponseDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                Category = setting.Category,
                DataType = setting.DataType,
                IsEditable = setting.IsEditable,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt
            };
        }
    }
}
