using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Drivers.Commands.UpdateDriver
{
    public class UpdateDriverHandler : IRequestHandler<UpdateDriverCommand, Unit>
    {
        private readonly IDriverRepository _repository;

        public UpdateDriverHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Conductor no encontrado");

            var email = string.IsNullOrWhiteSpace(request.Email) ? null : Email.Create(request.Email);
            driver.SetName(request.FirstName, request.LastName);
            driver.SetDocument(request.DocumentType, request.DocumentNumber);
            driver.UpdateLicense(new LicenseNumber(request.LicenseNumber));
            driver.UpdateContact(PhoneNumber.Create(request.Phone), Address.Create(request.Street, request.City), email);
            driver.UpdatePhoto(request.PhotoUrl);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
