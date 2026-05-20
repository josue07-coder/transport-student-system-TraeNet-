using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Stops.Commands.UpdateStop
{
    public class UpdateStopHandler : IRequestHandler<UpdateStopCommand, Unit>
    {
        private readonly IStopRepository _repository;

        public UpdateStopHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateStopCommand request, CancellationToken cancellationToken)
        {
            var stop = await _repository.GetByIdAsync(request.Id)
                ?? throw new DomainException("Parada no encontrada");

            stop.SetName(request.Name);
            stop.UpdateLocation(
                Address.Create(request.Street, request.City),
                Coordinates.Create(request.Latitude, request.Longitude));
            stop.ChangeSector(request.SectorId);

            await _repository.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
