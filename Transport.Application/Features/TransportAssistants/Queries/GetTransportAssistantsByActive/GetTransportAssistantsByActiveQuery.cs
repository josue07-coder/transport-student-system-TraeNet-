using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantsByActive
{
    public record GetTransportAssistantsByActiveQuery(bool IsActive) : IRequest<List<TransportAssistantResponseDto>>;
}
