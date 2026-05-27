using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Grade grade)
        {
            await _context.Grades.AddAsync(grade);
        }

        public async Task<Grade?> GetByIdAsync(Guid id)
        {
            return await _context.Grades.FindAsync(id);
        }

        public async Task<List<Grade>> GetAllAsync()
        {
            return await _context.Grades.ToListAsync();
        }

        public async Task<PaginatedResponse<Grade>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Grades.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Grade>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Grade>> GetBySchoolAsync(Guid schoolId)
        {
            return await _context.Grades
                .Where(g => g.SchoolId == schoolId)
                .ToListAsync();
        }

        public async Task<bool> HasStudentsAsync(Guid gradeId)
        {
            return await _context.Students
                .AnyAsync(s => s.GradeId == gradeId);
        }

        public async Task<bool> ExistsByNameInSchoolAsync(string name, Guid schoolId, Guid? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Grades
                .AnyAsync(grade =>
                    grade.SchoolId == schoolId &&
                    grade.Name.ToLower() == normalizedName &&
                    (!excludeId.HasValue || grade.Id != excludeId.Value));
        }

        public async Task<bool> BelongsToSchoolAsync(Guid gradeId, Guid schoolId)
        {
            return await _context.Grades
                .AnyAsync(grade => grade.Id == gradeId && grade.SchoolId == schoolId);
        }

        public async Task<bool> HasActiveStudentsAsync(Guid gradeId)
        {
            return await _context.Students
                .AnyAsync(student => student.GradeId == gradeId);
        }

        public void Delete(Grade grade)
        {
            _context.Grades.Remove(grade);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
