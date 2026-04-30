
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IRouteAssignmentRepository
    {
        Task<RouteAssignment?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
