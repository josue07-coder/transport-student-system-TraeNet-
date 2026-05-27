using Transport.Application.Common.Pagination;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Interfaces
{
    public interface IVehicleRepository
    {
        Task AddAsync(Vehicle vehicle);
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);
        Task<List<Vehicle>> GetByStatusAsync(VehicleStatus status);
        Task<PaginatedResponse<Vehicle>> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByPlateAsync(string plateNumber, Guid? excludeId = null);
        Task<bool> HasInProgressTripAsync(Guid vehicleId);
        Task<bool> HasActiveRouteAssignmentAsync(Guid vehicleId);
        Task<int> GetMaxAssignedStudentCountAsync(Guid vehicleId);
        Task SaveChangesAsync();
    }
}
