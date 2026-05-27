using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface IVisibilityService
    {
        Task<User> GetCurrentUserAsync();
        Task EnsureCanViewStudentAsync(Student student);
        Task EnsureCanViewRouteAssignmentAsync(RouteAssignment assignment);
        Task EnsureCanViewTripAsync(Trip trip);
        Task<List<Student>> FilterStudentsAsync(IEnumerable<Student> students);
        Task<List<RouteAssignment>> FilterRouteAssignmentsAsync(IEnumerable<RouteAssignment> assignments);
        Task<List<Trip>> FilterTripsAsync(IEnumerable<Trip> trips);
        Task<List<Route>> FilterRoutesAsync(IEnumerable<Route> routes, Func<Guid, Task<List<RouteAssignment>>> getAssignmentsByRouteAsync);
    }
}
