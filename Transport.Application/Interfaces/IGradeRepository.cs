using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IGradeRepository
    {
        Task AddAsync(Grade grade);
        Task<Grade?> GetByIdAsync(Guid id);
        Task<List<Grade>> GetAllAsync();
        Task<PaginatedResponse<Grade>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Grade>> GetBySchoolAsync(Guid schoolId);
        Task<bool> HasStudentsAsync(Guid gradeId);
        Task<bool> ExistsByNameInSchoolAsync(string name, Guid schoolId, Guid? excludeId = null);
        Task<bool> BelongsToSchoolAsync(Guid gradeId, Guid schoolId);
        Task<bool> HasActiveStudentsAsync(Guid gradeId);
        void Delete(Grade grade);
        Task SaveChangesAsync();
    }
}
