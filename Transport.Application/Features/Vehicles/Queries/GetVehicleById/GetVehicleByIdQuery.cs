using MediatR;
using Transport.Application.Features.Vehicles.DTOs;

namespace Transport.Application.Features.Vehicles.Queries.GetVehicleById
{
    public record GetVehicleByIdQuery(Guid Id) : IRequest<VehicleDetailDto>;
}
