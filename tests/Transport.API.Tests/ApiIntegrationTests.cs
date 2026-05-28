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

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "Admin123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await ReadTokenAsync(response);
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Users_WithNonAdminToken_ReturnsForbidden()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "guardian-test", "Guardian123");

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReportsDashboard_WithAdminToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/reports/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Notifications_WithToken_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await AuthorizeAsync(client, "admin", "Admin123");

        var response = await client.GetAsync("/api/notifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task AuthorizeAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
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
