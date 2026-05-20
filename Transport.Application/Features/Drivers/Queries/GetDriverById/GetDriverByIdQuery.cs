using MediatR;
using Transport.Application.Features.Drivers.DTOs;

namespace Transport.Application.Features.Drivers.Queries.GetDriverById
{
    public record GetDriverByIdQuery(Guid Id) : IRequest<DriverDetailDto>;
}
