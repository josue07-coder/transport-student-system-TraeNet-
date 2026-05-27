using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly AppDbContext _context;

        public SchoolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(School school)
        {
            await _context.Schools.AddAsync(school);
        }

        public async Task<School?> GetByIdAsync(Guid id)
        {
            return await _context.Schools.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Schools
                .IgnoreQueryFilters()
                .AnyAsync(school => school.Id == id);
        }

        public async Task<bool> IsActiveAsync(Guid id)
        {
            return await _context.Schools
                .IgnoreQueryFilters()
                .AnyAsync(school => school.Id == id && school.IsActive);
        }

        public async Task<bool> ExistsByNameInSectorAsync(string name, Guid sectorId, Guid? excludeId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Schools
                .IgnoreQueryFilters()
                .AnyAsync(school =>
                    school.SectorId == sectorId &&
                    school.Name.ToLower() == normalizedName &&
                    (!excludeId.HasValue || school.Id != excludeId.Value));
        }

        public async Task<bool> HasActiveStudentsAsync(Guid schoolId)
        {
            return await _context.Students
                .AnyAsync(student => student.SchoolId == schoolId);
        }

        public async Task<bool> HasActiveRoutesAsync(Guid schoolId)
        {
            return await _context.Routes
                .AnyAsync(route => route.SchoolId == schoolId && route.Status == RouteStatus.Active);
        }

        public async Task<List<School>> GetAllAsync()
        {
            return await _context.Schools.ToListAsync();
        }

        public async Task<PaginatedResponse<School>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Schools.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<School>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<School>> GetBySectorAsync(Guid sectorId)
        {
            return await _context.Schools
                .Where(s => s.SectorId == sectorId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
