using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Drivers.Commands.CreateDriver
{
    public class CreateDriverHandler : IRequestHandler<CreateDriverCommand, Guid>
    {
        private readonly IDriverRepository _repository;

        public CreateDriverHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
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

            await _repository.AddAsync(driver);
            await _repository.SaveChangesAsync();

            return driver.Id;
        }
    }
}
