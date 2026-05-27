using Transport.Domain.Enums;

namespace Transport.Application.Features.Notifications.DTOs
{
    public class NotificationResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? RelatedEntityType { get; set; }
        public string? RelatedEntityId { get; set; }
    }
}
