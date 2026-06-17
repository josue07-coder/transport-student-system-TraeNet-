using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface INonSchoolDayRepository
    {
        Task AddAsync(NonSchoolDay nonSchoolDay);
        Task<NonSchoolDay?> GetByIdAsync(Guid id);
        Task<NonSchoolDay?> GetByIdWithSchoolAsync(Guid id);
        Task<PaginatedResponse<NonSchoolDay>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<NonSchoolDay>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<bool> ExistsActiveAsync(DateOnly date, Guid? schoolId, Guid? excludeId = null);
        Task<NonSchoolDay?> GetActiveForDateAsync(DateOnly date, Guid? schoolId);
        Task SaveChangesAsync();
    }
}
