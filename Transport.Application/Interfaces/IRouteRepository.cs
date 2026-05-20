using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface IRouteRepository
    {
        Task AddAsync(Route route);
        Task<Route?> GetByIdAsync(Guid id);
        Task<Route?> GetByIdWithStopsAsync(Guid id);
        Task<PaginatedResponse<Route>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Route>> GetBySchoolAsync(Guid schoolId);
        Task<List<Route>> GetByStatusAsync(RouteStatus status);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
