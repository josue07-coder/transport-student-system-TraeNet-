using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Drivers.Commands.UpdateDriver
{
    public class UpdateDriverHandler : IRequestHandler<UpdateDriverCommand, Unit>
    {
        private readonly IDriverRepository _repository;
        private readonly IUserRepository _userRepository;

        public UpdateDriverHandler(IDriverRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Conductor no encontrado");

            var previousDocumentNumber = driver.DocumentNumber;
            var documentChanged = !string.Equals(
                previousDocumentNumber,
                request.DocumentNumber,
                StringComparison.OrdinalIgnoreCase);

            if (await _repository.ExistsByDocumentAsync(request.DocumentNumber, driver.Id))
                throw new DomainException("Ya existe un conductor con este documento");

            if (await _repository.ExistsByLicenseAsync(request.LicenseNumber, driver.Id))
                throw new DomainException("Ya existe un conductor con esta licencia");

            if (documentChanged && await _userRepository.ExistsByUsernameAsync(request.DocumentNumber))
                throw new DomainException("Ya existe un usuario con este documento");

            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            driver.SetName(request.FirstName, request.LastName);
            driver.SetDocument(request.DocumentType, request.DocumentNumber);
            driver.UpdateLicense(new LicenseNumber(request.LicenseNumber));
            driver.UpdateContact(PhoneNumber.Create(request.Phone), Address.Create(request.Street, request.City), email);
            driver.UpdatePhoto(request.PhotoUrl);

            if (documentChanged)
            {
                var user = await _userRepository.GetByDriverIdAsync(driver.Id)
                    ?? throw new DomainException("Usuario vinculado al conductor no encontrado");

                user.SetUsername(request.DocumentNumber);

                var previousPlaceholderEmail = $"{previousDocumentNumber}@driver.local";
                if (string.Equals(user.Email, previousPlaceholderEmail, StringComparison.OrdinalIgnoreCase))
                    user.SetEmail($"{request.DocumentNumber}@driver.local");
            }

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
