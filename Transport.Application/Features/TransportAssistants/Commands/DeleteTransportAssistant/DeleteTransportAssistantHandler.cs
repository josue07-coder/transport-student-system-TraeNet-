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

            if (await _repository.HasInProgressTripAsync(assistant.Id))
                throw new DomainException("No se puede desactivar el asistente porque tiene un viaje en progreso");

            if (await _repository.HasActiveRouteAssignmentAsync(assistant.Id))
                throw new DomainException("No se puede desactivar el asistente porque está asignado a una ruta activa");

            assistant.Deactivate();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
