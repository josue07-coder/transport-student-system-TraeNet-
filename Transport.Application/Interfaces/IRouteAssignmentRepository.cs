
using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IRouteAssignmentRepository
    {
        Task AddAsync(RouteAssignment assignment);
        Task<RouteAssignment?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<RouteAssignment>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<RouteAssignment>> GetByRouteAsync(Guid routeId);
        Task<List<RouteAssignment>> GetByDriverAsync(Guid driverId);
        Task<List<RouteAssignment>> GetByVehicleAsync(Guid vehicleId);
        Task<bool> HasTripsAsync(Guid id);
        void Delete(RouteAssignment assignment);
        Task SaveChangesAsync();
    }
}
