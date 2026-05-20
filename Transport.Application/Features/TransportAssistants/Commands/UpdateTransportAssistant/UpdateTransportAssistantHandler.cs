using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.TransportAssistants.Commands.UpdateTransportAssistant
{
    public class UpdateTransportAssistantHandler : IRequestHandler<UpdateTransportAssistantCommand, Unit>
    {
        private readonly ITransportAssistantRepository _repository;

        public UpdateTransportAssistantHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateTransportAssistantCommand request, CancellationToken cancellationToken)
        {
            var assistant = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asistente de transporte no encontrado");

            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            assistant.UpdateDocument(request.DocumentType, request.DocumentNumber);
            assistant.SetName(request.FirstName, request.LastName);
            assistant.UpdateContact(PhoneNumber.Create(request.Phone), Address.Create(request.Street, request.City), email);
            assistant.UpdatePhoto(request.PhotoUrl);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
