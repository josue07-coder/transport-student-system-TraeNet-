using MediatR;
using Transport.Application.Features.SystemSettings.DTOs;

namespace Transport.Application.Features.SystemSettings.Queries.GetSystemSettingById
{
    public record GetSystemSettingByIdQuery(Guid Id) : IRequest<SystemSettingResponseDto>;
}
