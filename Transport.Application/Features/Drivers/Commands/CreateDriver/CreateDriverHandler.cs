using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Drivers.Commands.CreateDriver
{
    public class CreateDriverHandler : IRequestHandler<CreateDriverCommand, Guid>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public CreateDriverHandler(
            IDriverRepository driverRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasherService passwordHasher)
        {
            _driverRepository = driverRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            if (await _driverRepository.ExistsByDocumentAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un conductor con este documento");

            if (await _driverRepository.ExistsByLicenseAsync(request.LicenseNumber))
                throw new DomainException("Ya existe un conductor con esta licencia");

            if (await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            var role = await _roleRepository.GetByNameAsync("Driver")
                ?? throw new DomainException("Rol Driver no encontrado");

            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            var driver = new Driver(
                request.FirstName,
                request.LastName,
                request.DocumentType,
                request.DocumentNumber,
                new LicenseNumber(request.LicenseNumber),
                PhoneNumber.Create(request.Phone),
                Address.Create(request.Street, request.City),
                email);

            driver.UpdatePhoto(request.PhotoUrl);

            var user = new User(
                request.DocumentNumber,
                $"{request.FirstName} {request.LastName}",
                email?.Value ?? $"{request.DocumentNumber}@driver.local",
                _passwordHasher.HashPassword(request.DocumentNumber),
                role.Id,
                driverId: driver.Id,
                profileImageUrl: request.PhotoUrl);

            await _driverRepository.AddAsync(driver);
            await _userRepository.AddAsync(user);
            await _driverRepository.SaveChangesAsync();

            return driver.Id;
        }
    }
}
