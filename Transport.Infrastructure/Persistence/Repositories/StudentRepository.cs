using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.School)
                .Include(s => s.Grade)
                .Include(s => s.Guardian)
                .Include(s => s.Assignments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetByIdIncludingInactiveAsync(Guid id)
        {
            return await _context.Students
                .IgnoreQueryFilters()
                .Include(s => s.School)
                .Include(s => s.Grade)
                .Include(s => s.Guardian)
                .Include(s => s.Assignments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetByCodeAsync(string code)
        {
            return await _context.Students
                .Include(s => s.School)
                .Include(s => s.Grade)
                .Include(s => s.Guardian)
                .FirstOrDefaultAsync(s => s.StudentCode.Value == code);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<PaginatedResponse<Student>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Students.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Student>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<List<Student>> GetByGradeAsync(Guid gradeId)
        {
            return await _context.Students
                .Where(s => s.GradeId == gradeId)
                .ToListAsync();
        }

        public async Task<List<Student>> GetBySchoolAsync(Guid schoolId)
        {
            return await _context.Students
                .Where(s => s.SchoolId == schoolId)
                .ToListAsync();
        }

        public async Task<List<Student>> GetByGuardianAsync(Guid guardianId)
        {
            return await _context.Students
                .Where(s => s.GuardianId == guardianId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Students
                .AnyAsync(s => s.StudentCode.Value == code);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Students
                .AnyAsync(student => student.Id == id);
        }

        public async Task<bool> HasActiveRouteAssignmentAsync(Guid studentId)
        {
            return await _context.StudentRouteAssignments
                .AnyAsync(studentAssignment =>
                    studentAssignment.StudentId == studentId &&
                    studentAssignment.RouteAssignment.Route.Status == RouteStatus.Active);
        }

        public async Task<bool> HasInProgressTripAsync(Guid studentId)
        {
            return await _context.StudentRouteAssignments
                .AnyAsync(studentAssignment =>
                    studentAssignment.StudentId == studentId &&
                    studentAssignment.RouteAssignment.Trips.Any(trip => trip.Status == TripStatus.InProgress));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
