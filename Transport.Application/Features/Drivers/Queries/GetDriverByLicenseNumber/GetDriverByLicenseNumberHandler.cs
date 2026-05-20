using MediatR;
using Transport.Application.Features.Drivers.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Drivers.Queries.GetDriverByLicenseNumber
{
    public class GetDriverByLicenseNumberHandler : IRequestHandler<GetDriverByLicenseNumberQuery, DriverDetailDto>
    {
        private readonly IDriverRepository _repository;

        public GetDriverByLicenseNumberHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<DriverDetailDto> Handle(GetDriverByLicenseNumberQuery request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByLicenseNumberAsync(request.LicenseNumber)
                ?? throw new DomainException("Conductor no encontrado");

            return DriverMappings.ToDetailDto(driver);
        }
    }
}
