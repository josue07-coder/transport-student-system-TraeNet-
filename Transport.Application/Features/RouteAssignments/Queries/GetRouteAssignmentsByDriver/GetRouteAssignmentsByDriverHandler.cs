using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByDriver
{
    public class GetRouteAssignmentsByDriverHandler : IRequestHandler<GetRouteAssignmentsByDriverQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetRouteAssignmentsByDriverHandler(IRouteAssignmentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByDriverQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByDriverAsync(request.DriverId);
            assignments = await _visibilityService.FilterRouteAssignmentsAsync(assignments);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
