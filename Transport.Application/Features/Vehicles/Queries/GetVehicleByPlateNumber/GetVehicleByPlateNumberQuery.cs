using MediatR;
using Transport.Application.Features.Vehicles.DTOs;

namespace Transport.Application.Features.Vehicles.Queries.GetVehicleByPlateNumber
{
    public record GetVehicleByPlateNumberQuery(string PlateNumber) : IRequest<VehicleDetailDto>;
}
