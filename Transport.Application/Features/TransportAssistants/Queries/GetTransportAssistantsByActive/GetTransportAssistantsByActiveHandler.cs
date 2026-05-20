using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;
using Transport.Application.Interfaces;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantsByActive
{
    public class GetTransportAssistantsByActiveHandler : IRequestHandler<GetTransportAssistantsByActiveQuery, List<TransportAssistantResponseDto>>
    {
        private readonly ITransportAssistantRepository _repository;

        public GetTransportAssistantsByActiveHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TransportAssistantResponseDto>> Handle(GetTransportAssistantsByActiveQuery request, CancellationToken cancellationToken)
        {
            var assistants = await _repository.GetByActiveAsync(request.IsActive);
            return assistants.Select(TransportAssistantMappings.ToResponseDto).ToList();
        }
    }
}
