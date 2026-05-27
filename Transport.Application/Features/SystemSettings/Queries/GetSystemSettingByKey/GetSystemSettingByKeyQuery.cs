using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingByKey
{
    public record GetSystemSettingByKeyQuery(string Key) : IRequest<SystemSettingResponseDto>;
}
