using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class SectorRepository : ISectorRepository
    {
        private readonly AppDbContext _context;

        public SectorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Sector sector)
        {
            await _context.Sectors.AddAsync(sector);
        }

        public async Task<Sector?> GetByIdAsync(Guid id)
        {
            return await _context.Sectors.FindAsync(id);
        }

        public async Task<List<Sector>> GetAllAsync()
        {
            return await _context.Sectors.ToListAsync();
        }

        public async Task<PaginatedResponse<Sector>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Sectors.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Sector>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> HasSchoolsAsync(Guid sectorId)
        {
            return await _context.Schools
                .AnyAsync(s => s.SectorId == sectorId);
        }

        public async Task<bool> HasGuardiansAsync(Guid sectorId)
        {
            return await _context.Guardians
                .AnyAsync(g => g.SectorId == sectorId);
        }

        public async Task<bool> HasStopsAsync(Guid sectorId)
        {
            return await _context.Stops
                .AnyAsync(s => s.SectorId == sectorId);
        }

        public void Delete(Sector sector)
        {
            _context.Sectors.Remove(sector);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
