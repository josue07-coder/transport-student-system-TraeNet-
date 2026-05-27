using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ISectorRepository
    {
        Task AddAsync(Sector sector);
        Task<Sector?> GetByIdAsync(Guid id);
        Task<List<Sector>> GetAllAsync();
        Task<PaginatedResponse<Sector>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> HasSchoolsAsync(Guid sectorId);
        Task<bool> HasGuardiansAsync(Guid sectorId);
        Task<bool> HasStopsAsync(Guid sectorId);
        void Delete(Sector sector);
        Task SaveChangesAsync();
    }
}
