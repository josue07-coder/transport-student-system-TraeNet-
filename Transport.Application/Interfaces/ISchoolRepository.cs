using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ISchoolRepository
    {
        Task AddAsync(School school);
        Task<School?> GetByIdAsync(Guid id);
        Task<List<School>> GetAllAsync();
        Task<PaginatedResponse<School>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<School>> GetBySectorAsync(Guid sectorId);
        Task SaveChangesAsync();
    }
}
