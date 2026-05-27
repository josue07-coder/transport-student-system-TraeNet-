using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Tracking
{
    internal static class TrackingAccess
    {
        public static async Task EnsureCanViewTripAsync(Trip trip, IVisibilityService visibilityService)
        {
            await visibilityService.EnsureCanViewTripAsync(trip);
        }

        public static async Task EnsureCanUpdateTripAsync(Trip trip, IVisibilityService visibilityService)
        {
            var user = await visibilityService.GetCurrentUserAsync();
            if (CanUpdateTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para actualizar la ubicación de este viaje");
        }

        public static async Task<List<Trip>> FilterTripsAsync(IEnumerable<Trip> trips, IVisibilityService visibilityService)
        {
            return await visibilityService.FilterTripsAsync(trips);
        }

        private static bool CanUpdateTrip(User user, Trip trip)
        {
            if (IsRole(user, "Admin") || IsRole(user, "Supervisor"))
                return true;

            var assignment = trip.RouteAssignment;

            if (IsRole(user, "Driver"))
                return user.DriverId.HasValue && assignment.DriverId == user.DriverId.Value;

            if (IsRole(user, "TransportAssistant"))
                return user.TransportAssistantId.HasValue && assignment.TransportAssistantId == user.TransportAssistantId.Value;

            return false;
        }

        private static bool IsRole(User user, string role)
        {
            return string.Equals(user.Role?.Name, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
