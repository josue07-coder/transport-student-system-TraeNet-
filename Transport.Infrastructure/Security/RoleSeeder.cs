using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Security
{
    public static class RoleSeeder
    {
        private static readonly (string Name, string Description)[] BaseRoles =
        {
            ("Admin", "Administrador del sistema"),
            ("Supervisor", "Supervisor de operaciones"),
            ("Guardian", "Tutor o responsable de estudiante"),
            ("Driver", "Conductor de transporte"),
            ("TransportAssistant", "Asistente de transporte")
        };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            foreach (var role in BaseRoles)
            {
                var exists = await context.Roles.AnyAsync(existing => existing.Name == role.Name);
                if (!exists)
                    await context.Roles.AddAsync(new Role(role.Name, role.Description));
            }

            await context.SaveChangesAsync();
        }
    }
}
