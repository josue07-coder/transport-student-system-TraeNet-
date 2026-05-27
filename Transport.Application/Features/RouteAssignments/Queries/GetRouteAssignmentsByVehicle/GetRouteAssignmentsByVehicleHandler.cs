using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByVehicle
{
    public class GetRouteAssignmentsByVehicleHandler : IRequestHandler<GetRouteAssignmentsByVehicleQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetRouteAssignmentsByVehicleHandler(IRouteAssignmentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByVehicleQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByVehicleAsync(request.VehicleId);
            assignments = await _visibilityService.FilterRouteAssignmentsAsync(assignments);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
