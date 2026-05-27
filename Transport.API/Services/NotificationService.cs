using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task NotifyUserAsync(
            Guid userId,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            try
            {
                await _notificationRepository.AddAsync(new Notification(
                    userId,
                    title,
                    message,
                    type,
                    priority,
                    relatedEntityType,
                    relatedEntityId));

                await _notificationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notification delivery failed for user {UserId}", userId);
            }
        }

        public async Task NotifyUsersAsync(
            IEnumerable<Guid> userIds,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            var distinctUserIds = userIds
                .Where(userId => userId != Guid.Empty)
                .Distinct()
                .ToList();

            if (!distinctUserIds.Any())
                return;

            try
            {
                foreach (var userId in distinctUserIds)
                {
                    await _notificationRepository.AddAsync(new Notification(
                        userId,
                        title,
                        message,
                        type,
                        priority,
                        relatedEntityType,
                        relatedEntityId));
                }

                await _notificationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notification delivery failed for {Count} users", distinctUserIds.Count);
            }
        }

        public async Task NotifyRoleAsync(
            string roleName,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Medium,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            try
            {
                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role is null)
                    return;

                var users = await _userRepository.GetByRoleAsync(role.Id);
                await NotifyUsersAsync(
                    users.Where(user => user.IsActive).Select(user => user.Id),
                    title,
                    message,
                    type,
                    priority,
                    relatedEntityType,
                    relatedEntityId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notification delivery failed for role {RoleName}", roleName);
            }
        }
    }
}
