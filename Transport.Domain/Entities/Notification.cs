using Transport.Domain.Common;
using Transport.Domain.Enums;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public NotificationPriority Priority { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public string? RelatedEntityType { get; private set; }
        public string? RelatedEntityId { get; private set; }

        private Notification() { } // EF

        public Notification(
            Guid userId,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User is required");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Notification title is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Notification message is required");

            UserId = userId;
            Title = title.Trim();
            Message = message.Trim();
            Type = type;
            Priority = priority;
            RelatedEntityType = string.IsNullOrWhiteSpace(relatedEntityType) ? null : relatedEntityType.Trim();
            RelatedEntityId = string.IsNullOrWhiteSpace(relatedEntityId) ? null : relatedEntityId.Trim();
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            SetUpdated();
        }

        public void MarkAsUnread()
        {
            IsRead = false;
            ReadAt = null;
            SetUpdated();
        }
    }
}
