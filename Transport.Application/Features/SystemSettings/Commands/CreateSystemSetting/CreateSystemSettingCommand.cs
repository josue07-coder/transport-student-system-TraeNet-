using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;

namespace Transport.Application.Features.SystemSettings.Commands.CreateSystemSetting
{
    public class CreateSystemSettingCommand : IRequest<SystemSettingResponseDto>
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsEditable { get; set; } = true;
    }
}
