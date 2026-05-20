using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IDriverRepository
    {
        Task AddAsync(Driver driver);
        Task<Driver?> GetByIdAsync(Guid id);
        Task<Driver?> GetByLicenseNumberAsync(string licenseNumber);
        Task<List<Driver>> GetByActiveAsync(bool isActive);
        Task<PaginatedResponse<Driver>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
