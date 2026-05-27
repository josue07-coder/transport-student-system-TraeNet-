using MediatR;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetIncidentsByTrip
{
    public class GetIncidentsByTripHandler : IRequestHandler<GetIncidentsByTripQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetIncidentsByTripHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetIncidentsByTripQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _repository.GetByTripAsync(request.TripId);
            var visible = await IncidentAccess.FilterAsync(incidents, _visibilityService);
            return visible.Select(incident => incident.ToResponseDto()).ToList();
        }
    }
}
