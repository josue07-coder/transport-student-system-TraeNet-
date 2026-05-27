using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetAllRouteAssignments
{
    public class GetAllRouteAssignmentsHandler : IRequestHandler<GetAllRouteAssignmentsQuery, PaginatedResponse<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetAllRouteAssignmentsHandler(IRouteAssignmentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<PaginatedResponse<RouteAssignmentResponseDto>> Handle(GetAllRouteAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var visibleAssignments = await _visibilityService.FilterRouteAssignmentsAsync(assignments.Items);
            var items = visibleAssignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();

            return new PaginatedResponse<RouteAssignmentResponseDto>(items, visibleAssignments.Count, assignments.PageNumber, assignments.PageSize);
        }
    }
}
