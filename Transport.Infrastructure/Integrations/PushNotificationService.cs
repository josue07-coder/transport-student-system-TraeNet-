using Microsoft.Extensions.Logging;
using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendPushAsync(Guid userId, string title, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock push notification sent to user {UserId}. Title: {Title}", userId, title);
            return Task.CompletedTask;
        }
    }
}
