using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;

namespace Transport.Application.Features.SystemSettings.Queries.GetAllSystemSettings
{
    public record GetAllSystemSettingsQuery : IRequest<List<SystemSettingResponseDto>>;
}
