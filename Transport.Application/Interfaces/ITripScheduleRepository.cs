using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface ITripScheduleRepository
    {
        Task AddAsync(TripSchedule schedule);
        Task<List<TripSchedule>> GetAllAsync();
        Task<TripSchedule?> GetByIdAsync(Guid id);
        Task<TripSchedule?> GetByIdWithDetailsAsync(Guid id);
        Task<TripSchedule?> GetByIdWithAssignmentDetailsAsync(Guid id);
        Task<List<TripSchedule>> GetByAssignmentAsync(Guid routeAssignmentId);
        Task<List<TripSchedule>> GetActiveByAssignmentAsync(Guid routeAssignmentId);
        Task<bool> ExistsOverlapAsync(
            Guid routeAssignmentId,
            TripDirection direction,
            DateOnly validFrom,
            DateOnly? validTo,
            bool monday,
            bool tuesday,
            bool wednesday,
            bool thursday,
            bool friday,
            bool saturday,
            bool sunday,
            Guid? excludeId = null);
        Task<bool> HasTripsAsync(Guid id);
        void Delete(TripSchedule schedule);
        Task SaveChangesAsync();
    }
}
