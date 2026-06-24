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
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ITripOperationAuthorizationService _operationAuthorizationService;

        public UpdateVehicleLocationHandler(
            ITripRepository tripRepository,
            IVehicleLocationRepository locationRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ISystemSettingService systemSettingService,
            ITripOperationAuthorizationService operationAuthorizationService)
        {
            _tripRepository = tripRepository;
            _locationRepository = locationRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _systemSettingService = systemSettingService;
            _operationAuthorizationService = operationAuthorizationService;
        }

        public async Task<Guid> Handle(UpdateVehicleLocationCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepository.GetByIdWithAssignmentDetailsAsync(request.TripId)
                ?? throw new DomainException("Viaje no encontrado");

            if (trip.Status != TripStatus.InProgress)
                throw new DomainException("Solo se puede registrar ubicación para un viaje en progreso");

            await _operationAuthorizationService.EnsureCanUpdateLocationAsync(trip);

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

            var speedAlertsEnabled = await _systemSettingService.GetBoolAsync("GPS.EnableSpeedAlerts", true);
            var speedLimit = await _systemSettingService.GetDecimalAsync("GPS.SpeedLimitKmH", 80);

            if (speedAlertsEnabled && request.Speed.HasValue && request.Speed.Value > speedLimit)
            {
                await _notificationService.NotifyRoleAsync(
                    "Supervisor",
                    "Velocidad elevada",
                    $"El vehiculo de la ruta {trip.RouteAssignment.Route.Name} reporto velocidad superior a {speedLimit} km/h.",
                    NotificationType.Security,
                    NotificationPriority.High,
                    "Trip",
                    trip.Id.ToString());
            }

            return location.Id;
        }
    }
}
