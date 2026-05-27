using Microsoft.Extensions.Logging;
using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock SMS sent to {PhoneNumber}. Length: {Length}", phoneNumber, message.Length);
            return Task.CompletedTask;
        }
    }
}
