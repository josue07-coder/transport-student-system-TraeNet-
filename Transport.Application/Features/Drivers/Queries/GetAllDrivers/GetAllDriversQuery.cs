using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Drivers.DTOs;

namespace Transport.Application.Features.Drivers.Queries.GetAllDrivers
{
    public class GetAllDriversQuery : PaginationRequest, IRequest<PaginatedResponse<DriverResponseDto>>
    {
    }
}
