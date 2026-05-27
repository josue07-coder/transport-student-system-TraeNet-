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
        private readonly ISystemSettingService _settings;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            ISystemSettingService settings,
            IEmailService emailService,
            ISmsService smsService,
            IWhatsAppService whatsAppService,
            IPushNotificationService pushNotificationService,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _settings = settings;
            _emailService = emailService;
            _smsService = smsService;
            _whatsAppService = whatsAppService;
            _pushNotificationService = pushNotificationService;
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
                await TrySendExternalChannelsAsync(userId, title, message);
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

                foreach (var userId in distinctUserIds)
                {
                    await TrySendExternalChannelsAsync(userId, title, message);
                }
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

        private async Task TrySendExternalChannelsAsync(Guid userId, string title, string message)
        {
            try
            {
                var user = await _userRepository.GetByIdWithLinkedProfilesAsync(userId);
                if (user is null)
                    return;

                if (await _settings.GetBoolAsync("Notifications.EnableEmail", false) && !string.IsNullOrWhiteSpace(user.Email))
                    await _emailService.SendEmailAsync(user.Email, title, message);

                var phone = ResolvePhone(user);

                if (await _settings.GetBoolAsync("Notifications.EnableSms", false) && !string.IsNullOrWhiteSpace(phone))
                    await _smsService.SendSmsAsync(phone, message);

                if (await _settings.GetBoolAsync("Notifications.EnableWhatsApp", false) && !string.IsNullOrWhiteSpace(phone))
                    await _whatsAppService.SendWhatsAppMessageAsync(phone, message);

                if (await _settings.GetBoolAsync("Notifications.EnablePush", false))
                    await _pushNotificationService.SendPushAsync(user.Id, title, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "External notification channel failed for user {UserId}", userId);
            }
        }

        private static string? ResolvePhone(User user)
        {
            if (user.Guardian is not null)
                return user.Guardian.Phone;

            if (user.Driver is not null)
                return user.Driver.Phone.Value;

            if (user.TransportAssistant is not null)
                return user.TransportAssistant.Phone.Value;

            return null;
        }
    }
}
