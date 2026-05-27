using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetMyReportedIncidents
{
    public class GetMyReportedIncidentsHandler : IRequestHandler<GetMyReportedIncidentsQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetMyReportedIncidentsHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetMyReportedIncidentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            var incidents = await _repository.GetByReportedByAsync(user.Id);
            return incidents.Select(incident => incident.ToResponseDto()).ToList();
        }
    }
}
