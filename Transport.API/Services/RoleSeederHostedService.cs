using Transport.Infrastructure.Security;

namespace Transport.API.Services
{
    public class RoleSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RoleSeederHostedService> _logger;

        public RoleSeederHostedService(IServiceProvider services, ILogger<RoleSeederHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await RoleSeeder.SeedAsync(_services);
                await SystemSettingSeeder.SeedAsync(_services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Startup seed failed. The API will continue running, but seeded data may be unavailable.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
