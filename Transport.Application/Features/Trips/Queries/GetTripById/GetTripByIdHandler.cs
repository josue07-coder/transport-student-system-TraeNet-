using MediatR;
using Transport.Application.Features.Trips.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Queries.GetTripById
{
    public class GetTripByIdHandler : IRequestHandler<GetTripByIdQuery, TripDetailDto>
    {
        private readonly ITripRepository _repository;
        private readonly IVisibilityService _visibilityService;

        public GetTripByIdHandler(ITripRepository repository, IVisibilityService visibilityService)
        {
            _repository = repository;
            _visibilityService = visibilityService;
        }

        public async Task<TripDetailDto> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            await _visibilityService.EnsureCanViewTripAsync(trip);

            return TripMappings.ToDetailDto(trip);
        }
    }
}
