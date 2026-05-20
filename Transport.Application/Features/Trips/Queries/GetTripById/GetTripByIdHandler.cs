using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Queries.GetTripById
{
    public class GetTripByIdHandler : IRequestHandler<GetTripByIdQuery, TripDetailDto>
    {
        private readonly ITripRepository _repository;

        public GetTripByIdHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<TripDetailDto> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            return TripMappings.ToDetailDto(trip);
        }
    }
}
