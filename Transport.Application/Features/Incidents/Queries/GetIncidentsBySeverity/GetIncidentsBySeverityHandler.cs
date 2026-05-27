using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsBySeverity
{
    public class GetIncidentsBySeverityHandler : IRequestHandler<GetIncidentsBySeverityQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetIncidentsBySeverityHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetIncidentsBySeverityQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _repository.GetBySeverityAsync(request.Severity);
            var visible = await IncidentAccess.FilterAsync(incidents, _visibilityService);
            return visible.Select(incident => incident.ToResponseDto()).ToList();
        }
    }
}
