using FluentAssertions;
using Moq;
using Transport.Application.Features.Auth.Commands.Login;
using Transport.Application.Interfaces;
using Transport.Application.Tests.Testing;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Auth;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_WithInvalidCredentials_Fails()
    {
        var repositories = CreateHandler();
        repositories.Users.Setup(x => x.GetByUsernameAsync("admin")).ReturnsAsync((User?)null);

        var act = () => repositories.Handler.Handle(new LoginCommand { Username = "admin", Password = "bad" }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_WithInactiveUser_Fails()
    {
        var role = new Role("Admin", "Administrator");
        var user = new User("admin", "Admin", "admin@test.local", "hash", role.Id);
        user.Deactivate();

        var repositories = CreateHandler();
        repositories.Users.Setup(x => x.GetByUsernameAsync("admin")).ReturnsAsync(user);

        var act = () => repositories.Handler.Handle(new LoginCommand { Username = "admin", Password = "Admin123" }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        var role = new Role("Admin", "Administrator");
        var user = new User("admin", "Admin", "admin@test.local", "hash", role.Id);
        ReflectionHelper.SetProperty(user, nameof(User.Role), role);

        var repositories = CreateHandler();
        repositories.Users.Setup(x => x.GetByUsernameAsync("admin")).ReturnsAsync(user);
        repositories.PasswordHasher.Setup(x => x.VerifyPassword("Admin123", "hash")).Returns(true);
        repositories.Jwt.Setup(x => x.GenerateToken(user, "Admin")).Returns("jwt-token");

        var response = await repositories.Handler.Handle(
            new LoginCommand { Username = "admin", Password = "Admin123" },
            CancellationToken.None);

        response.Token.Should().Be("jwt-token");
        response.Role.Should().Be("Admin");
        repositories.Users.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    private static LoginHandlerFixture CreateHandler()
    {
        var users = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasherService>();
        var jwt = new Mock<IJwtTokenService>();
        var audit = new Mock<IAuditService>();

        return new LoginHandlerFixture(
            new LoginHandler(users.Object, hasher.Object, jwt.Object, audit.Object),
            users,
            hasher,
            jwt);
    }

    private sealed record LoginHandlerFixture(
        LoginHandler Handler,
        Mock<IUserRepository> Users,
        Mock<IPasswordHasherService> PasswordHasher,
        Mock<IJwtTokenService> Jwt);
}
