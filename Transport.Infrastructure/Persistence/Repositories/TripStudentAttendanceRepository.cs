using Microsoft.EntityFrameworkCore;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class TripStudentAttendanceRepository : ITripStudentAttendanceRepository
    {
        private readonly AppDbContext _context;

        public TripStudentAttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<TripStudentAttendance> attendances)
        {
            await _context.TripStudentAttendances.AddRangeAsync(attendances);
        }

        public async Task<List<TripStudentAttendance>> GetByTripAsync(Guid tripId)
        {
            return await _context.TripStudentAttendances
                .Where(attendance => attendance.TripId == tripId)
                .OrderBy(attendance => attendance.StudentNameSnapshot)
                .ToListAsync();
        }

        public async Task<TripStudentAttendance?> GetByTripAndStudentAsync(Guid tripId, Guid studentId)
        {
            return await _context.TripStudentAttendances
                .FirstOrDefaultAsync(attendance => attendance.TripId == tripId && attendance.StudentId == studentId);
        }

        public async Task<bool> ExistsForTripAsync(Guid tripId)
        {
            return await _context.TripStudentAttendances.AnyAsync(attendance => attendance.TripId == tripId);
        }

        public async Task<bool> GuardianCanAccessTripAsync(Guid guardianId, Guid tripId)
        {
            return await _context.TripStudentAttendances.AnyAsync(attendance =>
                attendance.TripId == tripId &&
                attendance.GuardianIdSnapshot == guardianId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
