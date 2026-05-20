using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantById
{
    public record GetTransportAssistantByIdQuery(Guid Id) : IRequest<TransportAssistantDetailDto>;
}
