using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByStatus
{
    public class GetIncidentsByStatusHandler : IRequestHandler<GetIncidentsByStatusQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetIncidentsByStatusHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetIncidentsByStatusQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _repository.GetByStatusAsync(request.Status);
            var visible = await IncidentAccess.FilterAsync(incidents, _visibilityService);
            return visible.Select(incident => incident.ToResponseDto()).ToList();
        }
    }
}
