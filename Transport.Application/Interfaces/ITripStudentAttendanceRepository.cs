using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ITripStudentAttendanceRepository
    {
        Task AddRangeAsync(IEnumerable<TripStudentAttendance> attendances);
        Task<List<TripStudentAttendance>> GetByTripAsync(Guid tripId);
        Task<TripStudentAttendance?> GetByTripAndStudentAsync(Guid tripId, Guid studentId);
        Task<bool> ExistsForTripAsync(Guid tripId);
        Task<bool> GuardianCanAccessTripAsync(Guid guardianId, Guid tripId);
        Task SaveChangesAsync();
    }
}
