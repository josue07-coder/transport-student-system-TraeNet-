using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ITripRouteDeviationRepository
    {
        Task AddAsync(TripRouteDeviation deviation);
        Task<TripRouteDeviation?> GetByIdAsync(Guid id);
        Task<List<TripRouteDeviation>> GetByTripAsync(Guid tripId);
        Task SaveChangesAsync();
    }
}
