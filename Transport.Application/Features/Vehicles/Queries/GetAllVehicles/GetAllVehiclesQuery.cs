using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Vehicles.DTOs;

namespace Transport.Application.Features.Vehicles.Queries.GetAllVehicles
{
    public class GetAllVehiclesQuery : PaginationRequest, IRequest<PaginatedResponse<VehicleResponseDto>>
    {
    }
}
