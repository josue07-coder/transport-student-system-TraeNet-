using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.TransportAssistants.Commands.CreateTransportAssistant
{
    public class CreateTransportAssistantHandler : IRequestHandler<CreateTransportAssistantCommand, Guid>
    {
        private readonly ITransportAssistantRepository _repository;

        public CreateTransportAssistantHandler(ITransportAssistantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTransportAssistantCommand request, CancellationToken cancellationToken)
        {
            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            var assistant = new TransportAssistant(
                request.DocumentType,
                request.DocumentNumber,
                request.FirstName,
                request.LastName,
                PhoneNumber.Create(request.Phone),
                Address.Create(request.Street, request.City),
                email,
                request.PhotoUrl);

            await _repository.AddAsync(assistant);
            await _repository.SaveChangesAsync();

            return assistant.Id;
        }
    }
}
