using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(notification => notification.User)
                .FirstOrDefaultAsync(notification => notification.Id == id);
        }

        public async Task<PaginatedResponse<Notification>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = NotificationQuery();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Notification>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PaginatedResponse<Notification>> GetByUserAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = NotificationQuery()
                .Where(notification => notification.UserId == userId);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Notification>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Notification>> GetUnreadAsync(Guid? userId = null)
        {
            var query = NotificationQuery()
                .Where(notification => !notification.IsRead);

            if (userId.HasValue)
                query = query.Where(notification => notification.UserId == userId.Value);

            return await query.ToListAsync();
        }

        public async Task MarkAllAsReadAsync(Guid? userId = null)
        {
            var query = _context.Notifications
                .Where(notification => !notification.IsRead);

            if (userId.HasValue)
                query = query.Where(notification => notification.UserId == userId.Value);

            var notifications = await query.ToListAsync();
            foreach (var notification in notifications)
                notification.MarkAsRead();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<Notification> NotificationQuery()
        {
            return _context.Notifications
                .Include(notification => notification.User)
                .OrderByDescending(notification => notification.CreatedAt)
                .AsQueryable();
        }
    }
}
