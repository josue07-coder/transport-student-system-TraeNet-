using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface ITripRepository
    {
        Task AddAsync(Trip trip);
        Task<Trip?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<Trip>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Trip>> GetByRouteAssignmentAsync(Guid routeAssignmentId);
        Task<List<Trip>> GetByStatusAsync(TripStatus status);
        Task<List<Trip>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Trip?> GetActiveByRouteAssignmentAsync(Guid routeAssignmentId);
        Task<bool> HasActiveTripAsync(Guid routeAssignmentId);
        Task SaveChangesAsync();
    }
}
