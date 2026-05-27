using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Incidents.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.Incidents.Queries.GetAllIncidents
{
    public class GetAllIncidentsHandler : IRequestHandler<GetAllIncidentsQuery, PaginatedResponse<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetAllIncidentsHandler(IIncidentRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<PaginatedResponse<IncidentResponseDto>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _repository.GetPagedAsync(request.PageNumber, request.PageSize);
            var visible = await IncidentAccess.FilterAsync(incidents.Items, _visibilityService);
            var items = visible.Select(incident => incident.ToResponseDto()).ToList();

            return new PaginatedResponse<IncidentResponseDto>(items, items.Count, incidents.PageNumber, incidents.PageSize);
        }
    }
}
