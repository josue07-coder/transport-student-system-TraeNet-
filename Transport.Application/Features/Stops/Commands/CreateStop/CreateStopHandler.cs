using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.ValueObjects;

namespace Transport.Application.Features.Stops.Commands.CreateStop
{
    public class CreateStopHandler : IRequestHandler<CreateStopCommand, Guid>
    {
        private readonly IStopRepository _repository;

        public CreateStopHandler(IStopRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateStopCommand request, CancellationToken cancellationToken)
        {
            var stop = new Stop(
                request.Name,
                Address.Create(request.Street, request.City),
                Coordinates.Create(request.Latitude, request.Longitude),
                request.SectorId);

            await _repository.AddAsync(stop);
            await _repository.SaveChangesAsync();

            return stop.Id;
        }
    }
}
