using Microsoft.Extensions.Logging;
using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly ILogger<WhatsAppService> _logger;

        public WhatsAppService(ILogger<WhatsAppService> logger)
        {
            _logger = logger;
        }

        public Task SendWhatsAppMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock WhatsApp message sent to {PhoneNumber}. Length: {Length}", phoneNumber, message.Length);
            return Task.CompletedTask;
        }
    }
}
