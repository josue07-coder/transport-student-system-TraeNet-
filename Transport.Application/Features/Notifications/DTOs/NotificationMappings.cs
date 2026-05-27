using Transport.Domain.Entities;

namespace Transport.Application.Features.Notifications.DTOs
{
    public static class NotificationMappings
    {
        public static NotificationResponseDto ToResponseDto(this Notification notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Priority = notification.Priority,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelatedEntityType = notification.RelatedEntityType,
                RelatedEntityId = notification.RelatedEntityId
            };
        }
    }
}
