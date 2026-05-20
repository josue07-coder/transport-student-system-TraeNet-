using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ITransportAssistantRepository
    {
        Task AddAsync(TransportAssistant assistant);
        Task<TransportAssistant?> GetByIdAsync(Guid id);
        Task<TransportAssistant?> GetByDocumentAsync(string documentNumber);
        Task<List<TransportAssistant>> GetByActiveAsync(bool isActive);
        Task<PaginatedResponse<TransportAssistant>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
