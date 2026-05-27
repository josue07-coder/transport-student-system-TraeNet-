using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.Incidents.DTOs;

namespace Transport.Application.Features.Incidents.Queries.GetAllIncidents
{
    public class GetAllIncidentsQuery : PaginationRequest, IRequest<PaginatedResponse<IncidentResponseDto>>
    {
    }
}
