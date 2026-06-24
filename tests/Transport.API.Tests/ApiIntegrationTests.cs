using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Transport.API.Tests.Testing;

namespace Transport.API.Tests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_WithAdminSeed_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = "admin", password = "Admin123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await ReadTokenAsync(response);
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_UsingLegacyUnversionedRoute_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "Admin123" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Login_UsingVersionedRoute_ReturnsToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = "admin", password = "Admin123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("api-supported-versions", out var versions).Should().BeTrue();
        versions.Should().Contain("1.0");

        var token = await ReadTokenAsync(response);
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/v1/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Users_WithNonAdminToken_ReturnsForbidden()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "guardian-test", "Guardian123");

        var response = await client.GetAsync("/api/v1/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReportsDashboard_WithAdminToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/v1/reports/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Notifications_WithToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/v1/notifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSector_WithAdminToken_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var response = await client.PostAsJsonAsync("/api/v1/sectors", new
        {
            name = $"Sector Versionado {suffix}",
            province = "Prueba",
            city = "Prueba"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Response_IncludesCorrelationIdHeader()
    {
        var client = _factory.CreateClient();
        const string correlationId = "qa4-correlation-test";
        client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

        var response = await client.GetAsync("/api/v1/me");

        response.Headers.TryGetValues("X-Correlation-ID", out var values).Should().BeTrue();
        values.Should().Contain(correlationId);
    }

    [Fact]
    public async Task DomainError_ResponseIncludesCorrelationIdAndDoesNotExposeServerDetails()
    {
        var client = _factory.CreateClient();
        const string correlationId = "qa4-domain-error-test";
        client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = "missing-user", password = "bad-password" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.TryGetValues("X-Correlation-ID", out var values).Should().BeTrue();
        values.Should().Contain(correlationId);

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        document.RootElement.GetProperty("correlationId").GetString().Should().Be(correlationId);
        document.RootElement.GetProperty("traceId").GetString().Should().Be(correlationId);
        document.RootElement.GetProperty("error").GetString().Should().NotContain("Exception");
    }

    [Theory]
    [InlineData("driver-test", "Driver123")]
    [InlineData("assistant-test", "Assistant123")]
    [InlineData("guardian-test", "Guardian123")]
    public async Task OperationalRoles_CannotAccessAdministrativeStudents(string username, string password)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, username, password);

        var response = await client.GetAsync("/api/v1/students");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("driver-test", "Driver123")]
    [InlineData("assistant-test", "Assistant123")]
    [InlineData("guardian-test", "Guardian123")]
    public async Task OperationalRoles_CannotAccessAdministrativeRoutes(string username, string password)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, username, password);

        var response = await client.GetAsync("/api/v1/routes");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Assistant_CannotCancelTrip()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "assistant-test", "Assistant123");

        var response = await client.PutAsJsonAsync($"/api/v1/trips/{Guid.NewGuid()}/cancel", new { reason = "No permitido" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Guardian_CannotModifyPassengerAttendance()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "guardian-test", "Guardian123");

        var response = await client.PutAsync($"/api/v1/trips/{Guid.NewGuid()}/students/{Guid.NewGuid()}/boarded", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("driver-test", "Driver123")]
    [InlineData("assistant-test", "Assistant123")]
    public async Task DriverAndAssistant_CanAccessMeTrips(string username, string password)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, username, password);

        var response = await client.GetAsync("/api/v1/me/trips");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Guardian_CanAccessMeStudents()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "guardian-test", "Guardian123");

        var response = await client.GetAsync("/api/v1/me/students");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("admin", "Admin123")]
    [InlineData("supervisor-test", "Supervisor123")]
    public async Task AdministrativeRoles_CanAccessStudentsAndRoutes(string username, string password)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, username, password);

        var students = await client.GetAsync("/api/v1/students");
        var routes = await client.GetAsync("/api/v1/routes");

        students.StatusCode.Should().Be(HttpStatusCode.OK);
        routes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/api/v1/me")]
    [InlineData("/api/v1/me/students")]
    [InlineData("/api/v1/me/route-assignments")]
    [InlineData("/api/v1/me/trips")]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/roles")]
    [InlineData("/api/v1/permissions")]
    [InlineData("/api/v1/sectors")]
    [InlineData("/api/v1/schools")]
    [InlineData("/api/v1/grades")]
    [InlineData("/api/v1/guardians")]
    [InlineData("/api/v1/students")]
    [InlineData("/api/v1/vehicles")]
    [InlineData("/api/v1/drivers")]
    [InlineData("/api/v1/transport-assistants")]
    [InlineData("/api/v1/stops")]
    [InlineData("/api/v1/routes")]
    [InlineData("/api/v1/route-assignments")]
    [InlineData("/api/v1/trips")]
    [InlineData("/api/v1/trip-schedules")]
    [InlineData("/api/v1/non-school-days")]
    [InlineData("/api/v1/notifications")]
    [InlineData("/api/v1/notifications/unread")]
    [InlineData("/api/v1/incidents")]
    [InlineData("/api/v1/incidents/my-reported")]
    [InlineData("/api/v1/tracking/active-trips")]
    [InlineData("/api/v1/reports/dashboard")]
    [InlineData("/api/v1/audit-logs")]
    [InlineData("/api/v1/system-settings")]
    [InlineData("/api/v1/backups")]
    [InlineData("/api/v1/backups/latest")]
    public async Task Admin_ReadEndpointsAcrossModules_ReturnOk(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync(path);

        ((int)response.StatusCode).Should().BeInRange(200, 204, path);
    }

    [Theory]
    [InlineData("/api/v1/reports/trips?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/reports/incidents?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/reports/drivers-performance?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/reports/vehicles-usage?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/reports/audit-summary?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/reports/attendance/low-presence?startDate=2026-01-01&endDate=2026-12-31&maximumPresencePercentage=80")]
    [InlineData("/api/v1/integrations/test-distance?originLat=18.2081&originLng=-71.1002&destinationLat=18.5001&destinationLng=-69.9886")]
    [InlineData("/api/v1/non-school-days/by-date-range?startDate=2026-01-01&endDate=2026-12-31")]
    [InlineData("/api/v1/non-school-days/active?date=2026-06-08")]
    public async Task Admin_QueryEndpointsAcrossModules_ReturnOk(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync(path);

        ((int)response.StatusCode).Should().BeInRange(200, 204, path);
    }

    [Theory]
    [InlineData("/api/v1/students/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/routes/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/route-assignments/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/trips/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/incidents/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/tracking/trips/00000000-0000-0000-0000-000000000001/current-location")]
    [InlineData("/api/v1/tracking/trips/00000000-0000-0000-0000-000000000001/history")]
    [InlineData("/api/v1/backups/00000000-0000-0000-0000-000000000001")]
    public async Task MissingResourceEndpoints_ReturnControlledClientError_NotServerError(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync(path);

        ((int)response.StatusCode).Should().BeInRange(400, 404, path);
    }

    [Theory]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/roles")]
    [InlineData("/api/v1/permissions")]
    [InlineData("/api/v1/reports/dashboard")]
    [InlineData("/api/v1/audit-logs")]
    [InlineData("/api/v1/system-settings")]
    [InlineData("/api/v1/backups")]
    [InlineData("/api/v1/integrations/test-distance?originLat=18.2081&originLng=-71.1002&destinationLat=18.5001&destinationLng=-69.9886")]
    public async Task Guardian_CannotAccessAdministrativeModules(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "guardian-test", "Guardian123");

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, path);
    }

    [Theory]
    [InlineData("/api/v1/sectors")]
    [InlineData("/api/v1/schools")]
    [InlineData("/api/v1/grades")]
    [InlineData("/api/v1/vehicles")]
    [InlineData("/api/v1/drivers")]
    [InlineData("/api/v1/transport-assistants")]
    [InlineData("/api/v1/stops")]
    [InlineData("/api/v1/routes")]
    [InlineData("/api/v1/route-assignments")]
    [InlineData("/api/v1/trip-schedules")]
    [InlineData("/api/v1/non-school-days")]
    public async Task Driver_CannotAccessCatalogAndManagementModules(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "driver-test", "Driver123");

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, path);
    }

    [Theory]
    [InlineData("/api/v1/sectors")]
    [InlineData("/api/v1/schools")]
    [InlineData("/api/v1/grades")]
    [InlineData("/api/v1/vehicles")]
    [InlineData("/api/v1/drivers")]
    [InlineData("/api/v1/transport-assistants")]
    [InlineData("/api/v1/stops")]
    [InlineData("/api/v1/routes")]
    [InlineData("/api/v1/route-assignments")]
    [InlineData("/api/v1/trip-schedules")]
    [InlineData("/api/v1/non-school-days")]
    public async Task Assistant_CannotAccessCatalogAndManagementModules(string path)
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "assistant-test", "Assistant123");

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, path);
    }

    [Theory]
    [InlineData("/api/v1/sectors")]
    [InlineData("/api/v1/students")]
    [InlineData("/api/v1/routes")]
    [InlineData("/api/v1/trips")]
    [InlineData("/api/v1/notifications")]
    [InlineData("/api/v1/reports/dashboard")]
    public async Task ProtectedEndpoints_WithoutToken_ReturnUnauthorized(string path)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, path);
    }

    private static async Task AuthorizeAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();
        var token = await ReadTokenAsync(response);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<string> ReadTokenAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.GetProperty("token").GetString()!;
    }
}


