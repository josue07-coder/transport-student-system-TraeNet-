using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByRouteAssignment
{
    public class GetIncidentsByRouteAssignmentHandler : IRequestHandler<GetIncidentsByRouteAssignmentQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetIncidentsByRouteAssignmentHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetIncidentsByRouteAssignmentQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _repository.GetByRouteAssignmentAsync(request.RouteAssignmentId);
            var visible = await IncidentAccess.FilterAsync(incidents, _visibilityService);
            return visible.Select(incident => incident.ToResponseDto()).ToList();
        }
    }
}
