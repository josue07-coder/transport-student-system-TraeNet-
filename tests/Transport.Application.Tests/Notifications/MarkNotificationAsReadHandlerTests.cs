using FluentAssertions;
using Moq;
using Transport.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Application.Tests.Notifications;

public class MarkNotificationAsReadHandlerTests
{
    [Fact]
    public async Task Handle_AllowsNotificationOwner()
    {
        var userId = Guid.NewGuid();
        var notification = CreateNotification(userId);
        var handler = CreateHandler(notification, userId, "Guardian");

        await handler.Handle(new MarkNotificationAsReadCommand { Id = notification.Id }, CancellationToken.None);

        notification.IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AllowsAdmin()
    {
        var notification = CreateNotification(Guid.NewGuid());
        var handler = CreateHandler(notification, Guid.NewGuid(), "Admin");

        await handler.Handle(new MarkNotificationAsReadCommand { Id = notification.Id }, CancellationToken.None);

        notification.IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_BlocksOtherUsers()
    {
        var notification = CreateNotification(Guid.NewGuid());
        var handler = CreateHandler(notification, Guid.NewGuid(), "Guardian");

        var act = () => handler.Handle(new MarkNotificationAsReadCommand { Id = notification.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    private static Notification CreateNotification(Guid userId)
    {
        return new Notification(userId, "Titulo", "Mensaje", NotificationType.System, NotificationPriority.Low);
    }

    private static MarkNotificationAsReadHandler CreateHandler(Notification notification, Guid currentUserId, string role)
    {
        var repository = new Mock<INotificationRepository>();
        var currentUser = new Mock<ICurrentUserService>();
        repository.Setup(x => x.GetByIdAsync(notification.Id)).ReturnsAsync(notification);
        currentUser.SetupGet(x => x.UserId).Returns(currentUserId);
        currentUser.SetupGet(x => x.Role).Returns(role);

        return new MarkNotificationAsReadHandler(repository.Object, currentUser.Object);
    }
}
