using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyUserAsync(
            Guid userId,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null);

        Task NotifyUsersAsync(
            IEnumerable<Guid> userIds,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null);

        Task NotifyRoleAsync(
            string roleName,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null);
    }
}
