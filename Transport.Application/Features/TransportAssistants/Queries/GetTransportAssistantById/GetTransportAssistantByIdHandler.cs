using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantById
{
    public class GetTransportAssistantByIdHandler : IRequestHandler<GetTransportAssistantByIdQuery, TransportAssistantDetailDto>
    {
        private readonly ITransportAssistantRepository _repository;

        public GetTransportAssistantByIdHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<TransportAssistantDetailDto> Handle(GetTransportAssistantByIdQuery request, CancellationToken cancellationToken)
        {
            var assistant = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asistente de transporte no encontrado");

            return TransportAssistantMappings.ToDetailDto(assistant);
        }
    }
}
