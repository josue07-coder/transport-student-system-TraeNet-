using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Students
                .AnyAsync(s => s.StudentCode.Value == code);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}