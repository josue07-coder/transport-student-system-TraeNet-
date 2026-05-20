using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transport.Application.Interfaces;
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

        private static readonly (string Name, string Description, string Module)[] BasePermissions =
        {
            ("Users.Read", "Ver usuarios", "Users"),
            ("Users.Update", "Actualizar usuarios", "Users"),
            ("Users.Deactivate", "Activar o desactivar usuarios", "Users"),
            ("Roles.Read", "Ver roles", "Roles"),
            ("Roles.Update", "Actualizar roles", "Roles"),
            ("Permissions.Read", "Ver permisos", "Permissions"),
            ("Education.Read", "Ver módulo educativo", "Education"),
            ("Education.Manage", "Administrar módulo educativo", "Education"),
            ("Transport.Read", "Ver módulo transporte", "Transport"),
            ("Transport.Manage", "Administrar módulo transporte", "Transport"),
            ("Trips.Read", "Ver viajes", "Trips"),
            ("Trips.Manage", "Administrar viajes", "Trips"),
            ("Reports.View", "Ver reportes", "Reports")
        };

        private static readonly Dictionary<string, string[]> RolePermissions = new()
        {
            ["Admin"] = BasePermissions.Select(permission => permission.Name).ToArray(),
            ["Supervisor"] =
            [
                "Users.Read",
                "Education.Read",
                "Education.Manage",
                "Transport.Read",
                "Transport.Manage",
                "Trips.Read",
                "Trips.Manage",
                "Reports.View"
            ],
            ["Guardian"] =
            [
                "Education.Read",
                "Trips.Read"
            ],
            ["Driver"] =
            [
                "Transport.Read",
                "Trips.Read",
                "Trips.Manage"
            ],
            ["TransportAssistant"] =
            [
                "Transport.Read",
                "Trips.Read"
            ]
        };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            foreach (var role in BaseRoles)
            {
                var exists = await context.Roles.AnyAsync(existing => existing.Name == role.Name);
                if (!exists)
                    await context.Roles.AddAsync(new Role(role.Name, role.Description));
            }

            await context.SaveChangesAsync();

            foreach (var permission in BasePermissions)
            {
                var exists = await context.Permissions.AnyAsync(existing => existing.Name == permission.Name);
                if (!exists)
                    await context.Permissions.AddAsync(new Permission(permission.Name, permission.Description, permission.Module));
            }

            await context.SaveChangesAsync();

            var roles = await context.Roles.ToListAsync();
            var permissions = await context.Permissions.ToListAsync();

            foreach (var assignment in RolePermissions)
            {
                var role = roles.FirstOrDefault(item => item.Name == assignment.Key);
                if (role is null)
                    continue;

                foreach (var permissionName in assignment.Value)
                {
                    var permission = permissions.FirstOrDefault(item => item.Name == permissionName);
                    if (permission is null)
                        continue;

                    var exists = await context.RolePermissions
                        .AnyAsync(item => item.RoleId == role.Id && item.PermissionId == permission.Id);

                    if (!exists)
                        await context.RolePermissions.AddAsync(new RolePermission(role.Id, permission.Id));
                }
            }

            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(role => role.Name == "Admin");
            if (adminRole is not null)
            {
                var adminExists = await context.Users
                    .IgnoreQueryFilters()
                    .AnyAsync(user => user.Username == "admin");

                if (!adminExists)
                {
                    await context.Users.AddAsync(new User(
                        username: "admin",
                        name: "Administrador",
                        email: "admin@system.local",
                        passwordHash: passwordHasher.HashPassword("Admin123"),
                        roleId: adminRole.Id));
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
