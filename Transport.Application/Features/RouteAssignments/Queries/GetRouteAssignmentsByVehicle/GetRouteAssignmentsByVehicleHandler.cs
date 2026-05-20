using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByVehicle
{
    public class GetRouteAssignmentsByVehicleHandler : IRequestHandler<GetRouteAssignmentsByVehicleQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;

        public GetRouteAssignmentsByVehicleHandler(IRouteAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByVehicleQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByVehicleAsync(request.VehicleId);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
