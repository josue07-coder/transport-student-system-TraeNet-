using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface IRouteRepository
    {
        Task AddAsync(Route route);
        void AddRouteStop(RouteStop routeStop);
        Task<Route?> GetByIdAsync(Guid id);
        Task<Route?> GetByIdWithStopsAsync(Guid id);
        Task<bool> IsActiveAsync(Guid id);
        Task<bool> HasStopsAsync(Guid id);
        Task<bool> HasActiveTripAsync(Guid routeId);
        Task<bool> ExistsByNameForSchoolAsync(string name, Guid schoolId, Guid? excludeRouteId = null);
        Task<PaginatedResponse<Route>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Route>> GetBySchoolAsync(Guid schoolId);
        Task<List<Route>> GetByStatusAsync(RouteStatus status);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
