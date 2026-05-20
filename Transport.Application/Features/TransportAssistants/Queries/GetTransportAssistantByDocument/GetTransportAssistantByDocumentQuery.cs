using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantByDocument
{
    public record GetTransportAssistantByDocumentQuery(string DocumentNumber) : IRequest<TransportAssistantDetailDto>;
}
