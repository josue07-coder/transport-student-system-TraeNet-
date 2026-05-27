using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.Incidents
{
    internal static class IncidentAccess
    {
        public static bool IsPrivileged(User user)
        {
            return IsRole(user, "Admin") || IsRole(user, "Supervisor");
        }

        public static bool CanResolve(User user, Incident incident)
        {
            return IsPrivileged(user) || incident.AssignedToUserId == user.Id;
        }

        public static async Task EnsureCanViewAsync(Incident incident, IVisibilityService visibilityService)
        {
            var user = await visibilityService.GetCurrentUserAsync();
            if (CanView(user, incident))
                return;

            throw new DomainException("No tiene permiso para consultar este incidente");
        }

        public static async Task<List<Incident>> FilterAsync(IEnumerable<Incident> incidents, IVisibilityService visibilityService)
        {
            var user = await visibilityService.GetCurrentUserAsync();
            return incidents.Where(incident => CanView(user, incident)).ToList();
        }

        private static bool CanView(User user, Incident incident)
        {
            if (IsPrivileged(user) || incident.ReportedByUserId == user.Id || incident.AssignedToUserId == user.Id)
                return true;

            if (IsRole(user, "Driver"))
                return user.DriverId.HasValue &&
                    (incident.DriverId == user.DriverId.Value ||
                     incident.RouteAssignment?.DriverId == user.DriverId.Value ||
                     incident.Trip?.RouteAssignment?.DriverId == user.DriverId.Value);

            if (IsRole(user, "TransportAssistant"))
                return user.TransportAssistantId.HasValue &&
                    (incident.TransportAssistantId == user.TransportAssistantId.Value ||
                     incident.RouteAssignment?.TransportAssistantId == user.TransportAssistantId.Value ||
                     incident.Trip?.RouteAssignment?.TransportAssistantId == user.TransportAssistantId.Value);

            if (IsRole(user, "Guardian"))
            {
                if (!user.GuardianId.HasValue)
                    return false;

                var guardianId = user.GuardianId.Value;
                return incident.RouteAssignment?.Students.Any(studentAssignment =>
                        studentAssignment.Student?.GuardianId == guardianId) == true ||
                    incident.Trip?.RouteAssignment?.Students.Any(studentAssignment =>
                        studentAssignment.Student?.GuardianId == guardianId) == true;
            }

            return false;
        }

        private static bool IsRole(User user, string role)
        {
            return string.Equals(user.Role?.Name, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
