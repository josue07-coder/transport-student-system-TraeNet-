namespace Transport.Application.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendWhatsAppMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}
