using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.CancelTrip
{
    public class CancelTripHandler : IRequestHandler<CancelTripCommand, Unit>
    {
        private readonly ITripRepository _repository;

        public CancelTripHandler(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Viaje no encontrado");

            trip.Cancel();
            await _repository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
