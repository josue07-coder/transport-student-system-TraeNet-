
using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IRouteAssignmentRepository
    {
        Task AddAsync(RouteAssignment assignment);
        Task<RouteAssignment?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<RouteAssignment>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<RouteAssignment>> GetByRouteAsync(Guid routeId);
        Task<List<RouteAssignment>> GetByDriverAsync(Guid driverId);
        Task<List<RouteAssignment>> GetByTransportAssistantAsync(Guid transportAssistantId);
        Task<List<RouteAssignment>> GetByGuardianAsync(Guid guardianId);
        Task<List<RouteAssignment>> GetByVehicleAsync(Guid vehicleId);
        Task<bool> HasTripsAsync(Guid id);
        Task<bool> HasDriverScheduleConflictAsync(Guid driverId, Guid routeId, Guid? excludedAssignmentId = null);
        Task<bool> HasVehicleScheduleConflictAsync(Guid vehicleId, Guid routeId, Guid? excludedAssignmentId = null);
        Task<bool> HasTransportAssistantScheduleConflictAsync(Guid transportAssistantId, Guid routeId, Guid? excludedAssignmentId = null);
        Task<bool> HasStudentScheduleConflictAsync(Guid studentId, Guid routeId, Guid? excludedAssignmentId = null);
        Task<int> CountStudentsAsync(Guid id);
        void Delete(RouteAssignment assignment);
        Task SaveChangesAsync();
    }
}
