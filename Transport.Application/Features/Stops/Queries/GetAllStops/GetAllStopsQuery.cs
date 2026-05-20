using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Stops.DTOs;

namespace Transport.Application.Features.Stops.Queries.GetAllStops
{
    public class GetAllStopsQuery : PaginationRequest, IRequest<PaginatedResponse<StopResponseDto>>
    {
    }
}
