using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.TransportAssistants.Commands.CreateTransportAssistant
{
    public class CreateTransportAssistantHandler : IRequestHandler<CreateTransportAssistantCommand, Guid>
    {
        private readonly ITransportAssistantRepository _assistantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public CreateTransportAssistantHandler(
            ITransportAssistantRepository assistantRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasherService passwordHasher)
        {
            _assistantRepository = assistantRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateTransportAssistantCommand request, CancellationToken cancellationToken)
        {
            if (await _assistantRepository.ExistsByDocumentAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un asistente de transporte con este documento");

            if (await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            var role = await _roleRepository.GetByNameAsync("TransportAssistant")
                ?? throw new DomainException("Rol TransportAssistant no encontrado");

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

            var user = new User(
                request.DocumentNumber,
                $"{request.FirstName} {request.LastName}",
                email?.Value ?? $"{request.DocumentNumber}@assistant.local",
                _passwordHasher.HashPassword(request.DocumentNumber),
                role.Id,
                transportAssistantId: assistant.Id,
                profileImageUrl: request.PhotoUrl);

            await _assistantRepository.AddAsync(assistant);
            await _userRepository.AddAsync(user);
            await _assistantRepository.SaveChangesAsync();

            return assistant.Id;
        }
    }
}
