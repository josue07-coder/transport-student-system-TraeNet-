using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Tracking.Commands.UpdateVehicleLocation
{
    public class UpdateVehicleLocationHandler : IRequestHandler<UpdateVehicleLocationCommand, Guid>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleLocationRepository _locationRepository;
        private readonly IVisibilityService _visibilityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public UpdateVehicleLocationHandler(
            ITripRepository tripRepository,
            IVehicleLocationRepository locationRepository,
            IVisibilityService visibilityService,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _visibilityService = visibilityService;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(UpdateVehicleLocationCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede registrar ubicación para un viaje en progreso");

            await TrackingAccess.EnsureCanUpdateTripAsync(trip, _visibilityService);

            var location = new VehicleLocation(
                trip.Id,
                trip.RouteAssignment.VehicleId,
                request.Latitude,
                request.Longitude,
                request.Speed,
                request.Heading,
                _currentUserService.UserId);

            await _locationRepository.AddAsync(location);
            await _locationRepository.SaveChangesAsync();

            if (request.Speed.HasValue && request.Speed.Value > 80)
            {
                await _notificationService.NotifyRoleAsync(
                    "Supervisor",
                    "Velocidad elevada",
                    $"El vehículo de la ruta {trip.RouteAssignment.Route.Name} reportó velocidad superior a 80 km/h.",
                    NotificationType.Security,
                    NotificationPriority.High,
                    "Trip",
                    trip.Id.ToString());
            }

            return location.Id;
        }
    }
}
