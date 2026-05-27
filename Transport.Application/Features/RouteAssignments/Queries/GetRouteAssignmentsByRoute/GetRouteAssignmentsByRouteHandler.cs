using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByRoute
{
    public class GetRouteAssignmentsByRouteHandler : IRequestHandler<GetRouteAssignmentsByRouteQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetRouteAssignmentsByRouteHandler(IRouteAssignmentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByRouteQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByRouteAsync(request.RouteId);
            assignments = await _visibilityService.FilterRouteAssignmentsAsync(assignments);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
