using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.RouteAssignments.DTOs;

namespace Transport.Application.Features.RouteAssignments.Queries.GetAllRouteAssignments
{
    public class GetAllRouteAssignmentsQuery : PaginationRequest, IRequest<PaginatedResponse<RouteAssignmentResponseDto>>
    {
    }
}
