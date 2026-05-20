using MediatR;
using Transport.Application.Features.Drivers.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Drivers.Queries.GetDriverById
{
    public class GetDriverByIdHandler : IRequestHandler<GetDriverByIdQuery, DriverDetailDto>
    {
        private readonly IDriverRepository _repository;

        public GetDriverByIdHandler(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<DriverDetailDto> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            var driver = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Conductor no encontrado");

            return DriverMappings.ToDetailDto(driver);
        }
    }
}
