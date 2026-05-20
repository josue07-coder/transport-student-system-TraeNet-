using MediatR;
using Transport.Application.Features.TransportAssistants.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TransportAssistants.Queries.GetTransportAssistantByDocument
{
    public class GetTransportAssistantByDocumentHandler : IRequestHandler<GetTransportAssistantByDocumentQuery, TransportAssistantDetailDto>
    {
        private readonly ITransportAssistantRepository _repository;

        public GetTransportAssistantByDocumentHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<TransportAssistantDetailDto> Handle(GetTransportAssistantByDocumentQuery request, CancellationToken cancellationToken)
        {
            var assistant = await _repository.GetByDocumentAsync(request.DocumentNumber)
                ?? throw new DomainException("Asistente de transporte no encontrado");

            return TransportAssistantMappings.ToDetailDto(assistant);
        }
    }
}
