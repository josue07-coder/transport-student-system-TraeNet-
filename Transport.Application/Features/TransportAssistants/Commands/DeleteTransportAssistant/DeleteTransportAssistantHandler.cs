using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TransportAssistants.Commands.DeleteTransportAssistant
{
    public class DeleteTransportAssistantHandler : IRequestHandler<DeleteTransportAssistantCommand, Unit>
    {
        private readonly ITransportAssistantRepository _repository;

        public DeleteTransportAssistantHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteTransportAssistantCommand request, CancellationToken cancellationToken)
        {
            var assistant = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asistente de transporte no encontrado");

            assistant.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
