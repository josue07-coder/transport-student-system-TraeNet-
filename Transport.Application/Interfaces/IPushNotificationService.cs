namespace Transport.Application.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendPushAsync(Guid userId, string title, string message, CancellationToken cancellationToken = default);
    }
}
