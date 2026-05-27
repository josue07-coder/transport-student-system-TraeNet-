using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.TransportAssistants.Commands.UpdateTransportAssistant
{
    public class UpdateTransportAssistantHandler : IRequestHandler<UpdateTransportAssistantCommand, Unit>
    {
        private readonly ITransportAssistantRepository _repository;
        private readonly IUserRepository _userRepository;

        public UpdateTransportAssistantHandler(
            ITransportAssistantRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateTransportAssistantCommand request, CancellationToken cancellationToken)
        {
            var assistant = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Asistente de transporte no encontrado");

            var previousDocumentNumber = assistant.DocumentNumber;
            var documentChanged = !string.Equals(
                previousDocumentNumber,
                request.DocumentNumber,
                StringComparison.OrdinalIgnoreCase);

            if (await _repository.ExistsByDocumentAsync(request.DocumentNumber, assistant.Id))
                throw new DomainException("Ya existe un asistente de transporte con este documento");

            if (documentChanged && await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            assistant.UpdateDocument(request.DocumentType, request.DocumentNumber);
            assistant.SetName(request.FirstName, request.LastName);
            assistant.UpdateContact(PhoneNumber.Create(request.Phone), Address.Create(request.Street, request.City), email);
            assistant.UpdatePhoto(request.PhotoUrl);

            if (documentChanged)
            {
                var user = await _userRepository.GetByTransportAssistantIdAsync(assistant.Id)
                    ?? throw new DomainException("Usuario vinculado al asistente de transporte no encontrado");

                user.SetUsername(request.DocumentNumber);

                var previousPlaceholderEmail = $"{previousDocumentNumber}@assistant.local";
                if (string.Equals(user.Email, previousPlaceholderEmail, StringComparison.OrdinalIgnoreCase))
                    user.SetEmail($"{request.DocumentNumber}@assistant.local");
            }

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
