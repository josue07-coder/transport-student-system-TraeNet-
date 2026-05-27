using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<Notification>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PaginatedResponse<Notification>> GetByUserAsync(Guid userId, int pageNumber, int pageSize);
        Task<List<Notification>> GetUnreadAsync(Guid? userId = null);
        Task MarkAllAsReadAsync(Guid? userId = null);
        Task SaveChangesAsync();
    }
}
