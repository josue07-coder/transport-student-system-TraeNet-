using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence;
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}