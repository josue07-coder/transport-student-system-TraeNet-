using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Common.Security
{
    public class VisibilityService : IVisibilityService
    {
        private const string AdminRole = "Admin";
        private const string SupervisorRole = "Supervisor";
        private const string GuardianRole = "Guardian";
        private const string DriverRole = "Driver";
        private const string TransportAssistantRole = "TransportAssistant";

        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public VisibilityService(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new DomainException("Usuario no autenticado");

            return await _userRepository.GetByIdWithLinkedProfilesAsync(userId)
                ?? throw new DomainException("Usuario autenticado no encontrado");
        }

        public async Task EnsureCanViewStudentAsync(Student student)
        {
            var user = await GetCurrentUserAsync();

            if (CanViewStudent(user, student))
                return;

            throw new DomainException("No tiene permiso para consultar este estudiante");
        }

        public async Task EnsureCanViewRouteAssignmentAsync(RouteAssignment assignment)
        {
            var user = await GetCurrentUserAsync();

            if (CanViewRouteAssignment(user, assignment))
                return;

            throw new DomainException("No tiene permiso para consultar esta asignación de ruta");
        }

        public async Task EnsureCanViewTripAsync(Trip trip)
        {
            var user = await GetCurrentUserAsync();

            if (CanViewTrip(user, trip))
                return;

            throw new DomainException("No tiene permiso para consultar este viaje");
        }

        public async Task<List<RouteAssignment>> FilterRouteAssignmentsAsync(IEnumerable<RouteAssignment> assignments)
        {
            var user = await GetCurrentUserAsync();
            return assignments.Where(assignment => CanViewRouteAssignment(user, assignment)).ToList();
        }

        public async Task<List<Student>> FilterStudentsAsync(IEnumerable<Student> students)
        {
            var user = await GetCurrentUserAsync();
            return students.Where(student => CanViewStudent(user, student)).ToList();
        }

        public async Task<List<Trip>> FilterTripsAsync(IEnumerable<Trip> trips)
        {
            var user = await GetCurrentUserAsync();
            return trips.Where(trip => CanViewTrip(user, trip)).ToList();
        }

        public async Task<List<Route>> FilterRoutesAsync(
            IEnumerable<Route> routes,
            Func<Guid, Task<List<RouteAssignment>>> getAssignmentsByRouteAsync)
        {
            var user = await GetCurrentUserAsync();

            if (IsPrivileged(user))
                return routes.ToList();

            var allowedRoutes = new List<Route>();
            foreach (var route in routes)
            {
                var assignments = await getAssignmentsByRouteAsync(route.Id);
                if (assignments.Any(assignment => CanViewRouteAssignment(user, assignment)))
                    allowedRoutes.Add(route);
            }

            return allowedRoutes;
        }

        private static bool CanViewStudent(User user, Student student)
        {
            if (IsPrivileged(user))
                return true;

            if (IsRole(user, GuardianRole))
                return user.GuardianId.HasValue && student.GuardianId == user.GuardianId.Value;

            return false;
        }

        private static bool CanViewTrip(User user, Trip trip)
        {
            if (IsPrivileged(user))
                return true;

            return trip.RouteAssignment != null && CanViewRouteAssignment(user, trip.RouteAssignment);
        }

        private static bool CanViewRouteAssignment(User user, RouteAssignment assignment)
        {
            if (IsPrivileged(user))
                return true;

            if (IsRole(user, DriverRole))
                return user.DriverId.HasValue && assignment.DriverId == user.DriverId.Value;

            if (IsRole(user, TransportAssistantRole))
                return user.TransportAssistantId.HasValue &&
                    assignment.TransportAssistantId == user.TransportAssistantId.Value;

            if (IsRole(user, GuardianRole))
            {
                return user.GuardianId.HasValue &&
                    assignment.Students.Any(studentAssignment =>
                        studentAssignment.Student?.GuardianId == user.GuardianId.Value);
            }

            return false;
        }

        private static bool IsPrivileged(User user)
        {
            return IsRole(user, AdminRole) || IsRole(user, SupervisorRole);
        }

        private static bool IsRole(User user, string role)
        {
            return string.Equals(user.Role?.Name, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
