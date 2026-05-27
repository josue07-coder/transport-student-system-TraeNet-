using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingsByCategory
{
    public record GetSystemSettingsByCategoryQuery(string Category) : IRequest<List<SystemSettingResponseDto>>;
}
