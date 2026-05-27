using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ITransportAssistantRepository
    {
        Task AddAsync(TransportAssistant assistant);
        Task<TransportAssistant?> GetByIdAsync(Guid id);
        Task<TransportAssistant?> GetByIdIncludingInactiveAsync(Guid id);
        Task<TransportAssistant?> GetByDocumentAsync(string documentNumber);
        Task<List<TransportAssistant>> GetByActiveAsync(bool isActive);
        Task<PaginatedResponse<TransportAssistant>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByDocumentAsync(string documentNumber, Guid? excludeId = null);
        Task<bool> HasInProgressTripAsync(Guid id);
        Task<bool> HasActiveRouteAssignmentAsync(Guid id);
        Task SaveChangesAsync();
    }
}
