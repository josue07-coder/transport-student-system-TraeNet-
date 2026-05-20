using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Trips.Commands.StartTrip
{
    public class StartTripHandler : IRequestHandler<StartTripCommand, Guid>
    {
        private readonly IRouteAssignmentRepository _assignmentRepository;
        private readonly ITripRepository _tripRepository;

        public StartTripHandler(IRouteAssignmentRepository assignmentRepository, ITripRepository tripRepository)
        {
            _assignmentRepository = assignmentRepository;
            _tripRepository = tripRepository;
        }

        public async Task<Guid> Handle(StartTripCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.RouteAssignmentId)
                ?? throw new DomainException("Asignación de ruta no encontrada");

            if (await _tripRepository.HasActiveTripAsync(request.RouteAssignmentId))
                throw new DomainException("Ya hay un viaje activo para esta asignación");

            var trip = assignment.StartTrip();
            await _tripRepository.AddAsync(trip);
            await _tripRepository.SaveChangesAsync();

            return trip.Id;
        }
    }
}
