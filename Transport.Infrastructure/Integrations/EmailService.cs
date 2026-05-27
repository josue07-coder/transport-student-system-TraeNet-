using Microsoft.Extensions.Logging;
using Transport.Application.Interfaces;

namespace Transport.Infrastructure.Integrations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Mock email sent to {To}. Subject: {Subject}", to, subject);
            return Task.CompletedTask;
        }
    }
}
