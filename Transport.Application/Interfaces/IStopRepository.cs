using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IStopRepository
    {
        Task AddAsync(Stop stop);
        Task<Stop?> GetByIdAsync(Guid id);
        Task<Stop?> GetByIdWithSectorAsync(Guid id);
        Task<List<Stop>> GetBySectorAsync(Guid sectorId);
        Task<List<Stop>> GetByCityAsync(string city);
        Task<PaginatedResponse<Stop>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasRouteStopsAsync(Guid stopId);
        void Delete(Stop stop);
        Task SaveChangesAsync();
    }
}
