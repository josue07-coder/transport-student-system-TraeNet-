using Transport.Infrastructure.Security;

namespace Transport.API.Services
{
    public class RoleSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RoleSeederHostedService> _logger;
        private readonly IWebHostEnvironment _environment;

        public RoleSeederHostedService(
            IServiceProvider services,
            ILogger<RoleSeederHostedService> logger,
            IWebHostEnvironment environment)
        {
            _services = services;
            _logger = logger;
            _environment = environment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await RoleSeeder.SeedAsync(_services);
                await SystemSettingSeeder.SeedAsync(_services);

                if (_environment.IsDevelopment())
                    await OperationalSeedSeeder.SeedAsync(_services);
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
