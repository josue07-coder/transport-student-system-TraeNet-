using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Common.Security
{
    public class TripOperationAuthorizationService : ITripOperationAuthorizationService
    {
        private const string AdminRole = "Admin";
        private const string SupervisorRole = "Supervisor";
        private const string GuardianRole = "Guardian";
        private const string DriverRole = "Driver";
        private const string TransportAssistantRole = "TransportAssistant";

        private readonly IVisibilityService _visibilityService;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;

        public TripOperationAuthorizationService(
            IVisibilityService visibilityService,
            ITripStudentAttendanceRepository attendanceRepository)
        {
            _visibilityService = visibilityService;
            _attendanceRepository = attendanceRepository;
        }

        public async Task EnsureCanViewTripAsync(Trip trip)
        {
            try
            {
                await _visibilityService.EnsureCanViewTripAsync(trip);
            }
            catch (DomainException)
            {
                if (await CurrentGuardianCanAccessTripSnapshotAsync(trip))
                    return;

                throw;
            }
        }

        public async Task EnsureCanStartTripAsync(RouteAssignment assignment)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (CanOperateAssignment(user, assignment))
                return;

            throw new DomainException("No tiene permiso para iniciar este viaje");
        }

        public async Task EnsureCanEndTripAsync(Trip trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (CanOperateTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para finalizar este viaje");
        }

        public async Task EnsureCanManageTripAttendanceAsync(Trip trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (CanOperateTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para actualizar pasajeros de este viaje");
        }

        public async Task EnsureCanReportRouteDeviationAsync(Trip trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (CanOperateTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para gestionar desvíos de este viaje");
        }

        public async Task EnsureCanUpdateLocationAsync(Trip trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (CanOperateTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para actualizar la ubicación de este viaje");
        }

        public async Task<bool> CanReportIncidentAsync(Trip? trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            if (trip is null || CanViewTrip(user, trip))
                return true;

            return await GuardianCanAccessTripSnapshotAsync(user, trip);
        }

        private static bool CanOperateTrip(User user, Trip trip)
        {
            return trip.RouteAssignment is not null && CanOperateAssignment(user, trip.RouteAssignment);
        }

        private static bool CanOperateAssignment(User user, RouteAssignment assignment)
        {
            if (IsPrivileged(user))
                return true;

            if (IsRole(user, DriverRole))
                return user.DriverId.HasValue && assignment.DriverId == user.DriverId.Value;

            if (IsRole(user, TransportAssistantRole))
                return user.TransportAssistantId.HasValue &&
                    assignment.TransportAssistantId == user.TransportAssistantId.Value;

            return false;
        }

        private static bool CanViewTrip(User user, Trip trip)
        {
            if (trip.RouteAssignment is null)
                return false;

            if (CanOperateAssignment(user, trip.RouteAssignment))
                return true;

            if (IsRole(user, GuardianRole))
            {
                return user.GuardianId.HasValue &&
                    trip.RouteAssignment.Students.Any(studentAssignment =>
                        studentAssignment.Student?.GuardianId == user.GuardianId.Value);
            }

            return false;
        }

        private async Task<bool> CurrentGuardianCanAccessTripSnapshotAsync(Trip trip)
        {
            var user = await _visibilityService.GetCurrentUserAsync();
            return await GuardianCanAccessTripSnapshotAsync(user, trip);
        }

        private async Task<bool> GuardianCanAccessTripSnapshotAsync(User user, Trip trip)
        {
            return IsRole(user, GuardianRole) &&
                user.GuardianId.HasValue &&
                await _attendanceRepository.GuardianCanAccessTripAsync(user.GuardianId.Value, trip.Id);
        }

        private static bool IsPrivileged(User user)
        {
            return IsRole(user, AdminRole) || IsRole(user, SupervisorRole);
        }

        private static bool IsRole(User user, string role)
        {
            return string.Equals(user.Role?.Name, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
