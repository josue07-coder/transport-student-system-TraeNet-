using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.EndTrip
{
    public class EndTripHandler : IRequestHandler<EndTripCommand, Unit>
    {
        private readonly ITripRepository _repository;

        public EndTripHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            trip.End();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
