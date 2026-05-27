using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface IIncidentRepository
    {
        Task AddAsync(Incident incident);
        Task AddCommentAsync(IncidentComment comment);
        Task<Incident?> GetByIdAsync(Guid id);
        Task<Incident?> GetByIdWithDetailsAsync(Guid id);
        Task<PaginatedResponse<Incident>> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<Incident>> GetByStatusAsync(IncidentStatus status);
        Task<List<Incident>> GetBySeverityAsync(IncidentSeverity severity);
        Task<List<Incident>> GetByTripAsync(Guid tripId);
        Task<List<Incident>> GetByRouteAssignmentAsync(Guid routeAssignmentId);
        Task<List<Incident>> GetByReportedByAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
