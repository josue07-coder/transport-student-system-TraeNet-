using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.API.Tests.Testing;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryDatabaseRoot _databaseRoot = new();
    private readonly string _databaseName = $"transport-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "TestConnection",
                ["Jwt:Key"] = "TransportStudentSystem_Development_Key_Change_For_Production_2026",
                ["Jwt:Issuer"] = "TransportStudentSystem",
                ["Jwt:Audience"] = "TransportStudentSystem",
                ["Jwt:ExpiresInMinutes"] = "60"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IHostedService>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName, _databaseRoot));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedUsers(scope.ServiceProvider, db);
        });
    }

    private static void SeedUsers(IServiceProvider services, AppDbContext db)
    {
        if (db.Users.Any())
            return;

        var adminRole = new Role("Admin", "Administrator");
        var supervisorRole = new Role("Supervisor", "Supervisor");
        var guardianRole = new Role("Guardian", "Guardian");
        var driverRole = new Role("Driver", "Driver");
        var assistantRole = new Role("TransportAssistant", "TransportAssistant");
        db.Roles.AddRange(adminRole, supervisorRole, guardianRole, driverRole, assistantRole);
        db.SaveChanges();

        var hasher = services.GetRequiredService<IPasswordHasherService>();
        db.Users.AddRange(
            new User("admin", "Admin", "admin@test.local", hasher.HashPassword("Admin123"), adminRole.Id),
            new User("supervisor-test", "Supervisor Test", "supervisor@test.local", hasher.HashPassword("Supervisor123"), supervisorRole.Id),
            new User("guardian-test", "Guardian Test", "guardian@test.local", hasher.HashPassword("Guardian123"), guardianRole.Id),
            new User("driver-test", "Driver Test", "driver@test.local", hasher.HashPassword("Driver123"), driverRole.Id, driverId: Guid.NewGuid()),
            new User("assistant-test", "Assistant Test", "assistant@test.local", hasher.HashPassword("Assistant123"), assistantRole.Id, transportAssistantId: Guid.NewGuid()));

        db.SaveChanges();
    }
}
