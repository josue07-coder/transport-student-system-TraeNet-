using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetAllRouteAssignments
{
    public class GetAllRouteAssignmentsHandler : IRequestHandler<GetAllRouteAssignmentsQuery, PaginatedResponse<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;

        public GetAllRouteAssignmentsHandler(IRouteAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<RouteAssignmentResponseDto>> Handle(GetAllRouteAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var items = assignments.Items.Select(RouteAssignmentMappings.ToResponseDto).ToList();

            return new PaginatedResponse<RouteAssignmentResponseDto>(items, assignments.TotalCount, assignments.PageNumber, assignments.PageSize);
        }
    }
}
