using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByDriver
{
    public class GetRouteAssignmentsByDriverHandler : IRequestHandler<GetRouteAssignmentsByDriverQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;

        public GetRouteAssignmentsByDriverHandler(IRouteAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByDriverQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByDriverAsync(request.DriverId);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
