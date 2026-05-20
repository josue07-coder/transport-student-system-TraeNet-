using MediatR;
using Transport.Application.Features.Drivers.DTOs;

namespace Transport.Application.Features.Drivers.Queries.GetDriversByActive
{
    public record GetDriversByActiveQuery(bool IsActive) : IRequest<List<DriverResponseDto>>;
}
