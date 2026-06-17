using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class NonSchoolDayRepository : INonSchoolDayRepository
    {
        private readonly AppDbContext _context;

        public NonSchoolDayRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NonSchoolDay nonSchoolDay)
        {
            await _context.NonSchoolDays.AddAsync(nonSchoolDay);
        }

        public async Task<NonSchoolDay?> GetByIdAsync(Guid id)
        {
            return await _context.NonSchoolDays.FirstOrDefaultAsync(day => day.Id == id);
        }

        public async Task<NonSchoolDay?> GetByIdWithSchoolAsync(Guid id)
        {
            return await DayQuery().FirstOrDefaultAsync(day => day.Id == id);
        }

        public async Task<PaginatedResponse<NonSchoolDay>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = DayQuery().OrderByDescending(day => day.Date);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<NonSchoolDay>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<NonSchoolDay>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await DayQuery()
                .Where(day => day.Date >= startDate && day.Date <= endDate)
                .OrderBy(day => day.Date)
                .ToListAsync();
        }

        public async Task<bool> ExistsActiveAsync(DateOnly date, Guid? schoolId, Guid? excludeId = null)
        {
            return await _context.NonSchoolDays.AnyAsync(day =>
                day.Id != excludeId &&
                day.IsActive &&
                day.Date == date &&
                day.SchoolId == schoolId);
        }

        public async Task<NonSchoolDay?> GetActiveForDateAsync(DateOnly date, Guid? schoolId)
        {
            return await DayQuery()
                .Where(day => day.IsActive && day.Date == date && (day.SchoolId == null || day.SchoolId == schoolId))
                .OrderByDescending(day => day.SchoolId.HasValue)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<NonSchoolDay> DayQuery()
        {
            return _context.NonSchoolDays
                .Include(day => day.School)
                .AsQueryable();
        }
    }
}
