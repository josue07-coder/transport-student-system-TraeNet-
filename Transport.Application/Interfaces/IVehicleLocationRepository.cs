using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IVehicleLocationRepository
    {
        Task AddAsync(VehicleLocation location);
        Task<VehicleLocation?> GetLatestByTripAsync(Guid tripId);
        Task<List<VehicleLocation>> GetHistoryByTripAsync(Guid tripId);
        Task<PaginatedResponse<VehicleLocation>> GetPagedAsync(int pageNumber, int pageSize);
        Task SaveChangesAsync();
    }
}
