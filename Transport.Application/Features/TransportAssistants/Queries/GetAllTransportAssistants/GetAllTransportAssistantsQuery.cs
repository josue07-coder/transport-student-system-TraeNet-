using MediatR;
using Transport.Application.Common.Pagination;
using Transport.Application.Features.TransportAssistants.DTOs;

namespace Transport.Application.Features.TransportAssistants.Queries.GetAllTransportAssistants
{
    public class GetAllTransportAssistantsQuery : PaginationRequest, IRequest<PaginatedResponse<TransportAssistantResponseDto>>
    {
    }
}
