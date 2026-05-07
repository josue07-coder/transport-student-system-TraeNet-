using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence;
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
        public async Task<List<School>> GetAllAsync()
        {
            return await _context.Schools.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}