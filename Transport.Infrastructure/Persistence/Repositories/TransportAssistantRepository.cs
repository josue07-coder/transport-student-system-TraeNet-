using Microsoft.EntityFrameworkCore;
using Transport.Application.Common.Pagination;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Enums;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Persistence.Repositories
{
    public class TransportAssistantRepository : ITransportAssistantRepository
    {
        private readonly AppDbContext _context;

        public TransportAssistantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TransportAssistant assistant)
        {
            await _context.TransportAssistants.AddAsync(assistant);
        }

        public async Task<TransportAssistant?> GetByIdAsync(Guid id)
        {
            return await _context.TransportAssistants
                .FirstOrDefaultAsync(assistant => assistant.Id == id);
        }

        public async Task<TransportAssistant?> GetByIdIncludingInactiveAsync(Guid id)
        {
            return await _context.TransportAssistants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(assistant => assistant.Id == id);
        }

        public async Task<TransportAssistant?> GetByDocumentAsync(string documentNumber)
        {
            return await _context.TransportAssistants
                .FirstOrDefaultAsync(assistant => assistant.DocumentNumber == documentNumber);
        }

        public async Task<List<TransportAssistant>> GetByActiveAsync(bool isActive)
        {
            return await _context.TransportAssistants
                .IgnoreQueryFilters()
                .Where(assistant => assistant.IsActive == isActive)
                .ToListAsync();
        }

        public async Task<PaginatedResponse<TransportAssistant>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.TransportAssistants.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<TransportAssistant>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.TransportAssistants
                .AnyAsync(assistant => assistant.Id == id);
        }

        public async Task<bool> ExistsByDocumentAsync(string documentNumber, Guid? excludeId = null)
        {
            var normalizedDocument = documentNumber.Trim().ToLower();

            return await _context.TransportAssistants
                .IgnoreQueryFilters()
                .AnyAsync(assistant =>
                    assistant.DocumentNumber.ToLower() == normalizedDocument &&
                    (!excludeId.HasValue || assistant.Id != excludeId.Value));
        }

        public async Task<bool> HasInProgressTripAsync(Guid id)
        {
            return await _context.Trips
                .AnyAsync(trip =>
                    trip.Status == TripStatus.InProgress &&
                    trip.RouteAssignment.TransportAssistantId == id);
        }

        public async Task<bool> HasActiveRouteAssignmentAsync(Guid id)
        {
            return await _context.RouteAssignments
                .AnyAsync(assignment =>
                    assignment.TransportAssistantId == id &&
                    assignment.Route.Status == RouteStatus.Active);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
