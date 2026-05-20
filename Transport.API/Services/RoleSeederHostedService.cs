using Transport.Infrastructure.Security;

namespace Transport.API.Services
{
    public class RoleSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _services;

        public RoleSeederHostedService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RoleSeeder.SeedAsync(_services);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
