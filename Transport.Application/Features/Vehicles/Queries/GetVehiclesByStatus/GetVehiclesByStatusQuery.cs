using MediatR;
using Transport.Application.Features.Vehicles.DTOs;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Vehicles.Queries.GetVehiclesByStatus
{
    public record GetVehiclesByStatusQuery(VehicleStatus Status) : IRequest<List<VehicleResponseDto>>;
}
