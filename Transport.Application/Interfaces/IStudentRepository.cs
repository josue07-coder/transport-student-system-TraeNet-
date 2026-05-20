using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student);
        Task<Student?> GetByIdAsync(Guid id);
        Task<Student?> GetByCodeAsync(string code);
        Task<List<Student>> GetAllAsync();
        Task<PaginatedResponse<Student>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Student>> GetByGradeAsync(Guid gradeId);
        Task<List<Student>> GetBySchoolAsync(Guid schoolId);
        Task<List<Student>> GetByGuardianAsync(Guid guardianId);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
