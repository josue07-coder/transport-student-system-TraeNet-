using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IGuardianRepository
    {
        Task AddAsync(Guardian guardian);
        Task<Guardian?> GetByIdAsync(Guid id);
        Task<Guardian?> GetByDocumentAsync(string documentNumber);
        Task<List<Guardian>> GetAllAsync();
        Task<PaginatedResponse<Guardian>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsByDocumentAsync(string documentNumber, Guid? excludeId = null);
        Task<bool> IsActiveAsync(Guid id);
        Task<bool> HasActiveStudentsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
