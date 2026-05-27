using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Features.Incidents
{
    internal static class IncidentNotificationHelper
    {
        public static async Task NotifyReportedAsync(
            Incident incident,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            await notificationService.NotifyRoleAsync(
                "Supervisor",
                "Incidente reportado",
                $"Se reportó el incidente: {incident.Title}.",
                NotificationType.Incident,
                ToNotificationPriority(incident.Severity),
                "Incident",
                incident.Id.ToString());

            if (incident.Severity == IncidentSeverity.Critical)
            {
                await notificationService.NotifyRoleAsync(
                    "Admin",
                    "Incidente crítico reportado",
                    $"Se reportó el incidente crítico: {incident.Title}.",
                    NotificationType.Incident,
                    NotificationPriority.Critical,
                    "Incident",
                    incident.Id.ToString());
            }

            await notificationService.NotifyUsersAsync(
                await GetRelatedUserIdsAsync(incident, userRepository),
                "Incidente relacionado",
                $"Se reportó un incidente relacionado a tu operación: {incident.Title}.",
                NotificationType.Incident,
                ToNotificationPriority(incident.Severity),
                "Incident",
                incident.Id.ToString());
        }

        public static async Task NotifyResolvedAsync(
            Incident incident,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            var userIds = await GetRelatedUserIdsAsync(incident, userRepository);
            userIds.Add(incident.ReportedByUserId);

            await notificationService.NotifyUsersAsync(
                userIds,
                "Incidente resuelto",
                $"El incidente {incident.Title} fue resuelto.",
                NotificationType.Incident,
                NotificationPriority.Medium,
                "Incident",
                incident.Id.ToString());
        }

        public static async Task NotifyCommentAddedAsync(
            Incident incident,
            Guid commenterUserId,
            INotificationService notificationService)
        {
            var userIds = new List<Guid>();

            if (incident.AssignedToUserId.HasValue && incident.AssignedToUserId.Value != commenterUserId)
                userIds.Add(incident.AssignedToUserId.Value);

            if (incident.ReportedByUserId != commenterUserId)
                userIds.Add(incident.ReportedByUserId);

            await notificationService.NotifyUsersAsync(
                userIds,
                "Comentario en incidente",
                $"Se agregó un comentario al incidente {incident.Title}.",
                NotificationType.Incident,
                NotificationPriority.Low,
                "Incident",
                incident.Id.ToString());
        }

        private static async Task<List<Guid>> GetRelatedUserIdsAsync(Incident incident, IUserRepository userRepository)
        {
            var userIds = new List<Guid>();
            var assignment = incident.RouteAssignment ?? incident.Trip?.RouteAssignment;

            if (assignment is null)
                return userIds;

            var driverUser = await userRepository.GetByDriverIdAsync(assignment.DriverId);
            if (driverUser is not null)
                userIds.Add(driverUser.Id);

            if (assignment.TransportAssistantId.HasValue)
            {
                var assistantUser = await userRepository.GetByTransportAssistantIdAsync(assignment.TransportAssistantId.Value);
                if (assistantUser is not null)
                    userIds.Add(assistantUser.Id);
            }

            foreach (var guardianId in assignment.Students
                .Select(studentAssignment => studentAssignment.Student.GuardianId)
                .Distinct())
            {
                var guardianUser = await userRepository.GetByGuardianIdAsync(guardianId);
                if (guardianUser is not null)
                    userIds.Add(guardianUser.Id);
            }

            return userIds.Distinct().ToList();
        }

        private static NotificationPriority ToNotificationPriority(IncidentSeverity severity)
        {
            return severity switch
            {
                IncidentSeverity.Low => NotificationPriority.Low,
                IncidentSeverity.Medium => NotificationPriority.Medium,
                IncidentSeverity.High => NotificationPriority.High,
                IncidentSeverity.Critical => NotificationPriority.Critical,
                _ => NotificationPriority.Medium
            };
        }
    }
}
