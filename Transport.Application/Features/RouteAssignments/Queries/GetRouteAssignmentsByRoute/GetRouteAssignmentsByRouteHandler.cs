using MediatR;
using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.RouteAssignments.Queries.GetRouteAssignmentsByRoute
{
    public class GetRouteAssignmentsByRouteHandler : IRequestHandler<GetRouteAssignmentsByRouteQuery, List<RouteAssignmentResponseDto>>
    {
        private readonly IRouteAssignmentRepository _repository;

        public GetRouteAssignmentsByRouteHandler(IRouteAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RouteAssignmentResponseDto>> Handle(GetRouteAssignmentsByRouteQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _repository.GetByRouteAsync(request.RouteId);
            return assignments.Select(RouteAssignmentMappings.ToResponseDto).ToList();
        }
    }
}
