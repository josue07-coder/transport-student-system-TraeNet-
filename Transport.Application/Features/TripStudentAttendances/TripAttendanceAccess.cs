using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances
{
    internal static class TripAttendanceAccess
    {
        private const string AdminRole = "Admin";
        private const string SupervisorRole = "Supervisor";
        private const string GuardianRole = "Guardian";
        private const string DriverRole = "Driver";
        private const string TransportAssistantRole = "TransportAssistant";

        public static async Task EnsureCanViewPassengersAsync(
            Trip trip,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            ITripStudentAttendanceRepository attendanceRepository)
        {
            var user = await GetCurrentUserAsync(userRepository, currentUserService);

            if (IsPrivileged(user) || IsAssignedDriver(user, trip) || IsAssignedAssistant(user, trip))
                return;

            if (IsRole(user, GuardianRole) &&
                user.GuardianId.HasValue &&
                await attendanceRepository.GuardianCanAccessTripAsync(user.GuardianId.Value, trip.Id))
            {
                return;
            }

            throw new DomainException("No tiene permiso para consultar los pasajeros de este viaje");
        }

        public static async Task EnsureCanUpdatePassengersAsync(
            Trip trip,
            IUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            var user = await GetCurrentUserAsync(userRepository, currentUserService);

            if (IsPrivileged(user) || IsAssignedDriver(user, trip) || IsAssignedAssistant(user, trip))
                return;

            throw new DomainException("No tiene permiso para actualizar pasajeros de este viaje");
        }

        private static async Task<User> GetCurrentUserAsync(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            var userId = currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            return await userRepository.GetByIdWithLinkedProfilesAsync(userId)
                ?? throw new DomainException("Usuario autenticado no encontrado");
        }

        private static bool IsAssignedDriver(User user, Trip trip)
        {
            return IsRole(user, DriverRole) &&
                user.DriverId.HasValue &&
                trip.RouteAssignment.DriverId == user.DriverId.Value;
        }

        private static bool IsAssignedAssistant(User user, Trip trip)
        {
            return IsRole(user, TransportAssistantRole) &&
                user.TransportAssistantId.HasValue &&
                trip.RouteAssignment.TransportAssistantId == user.TransportAssistantId.Value;
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
