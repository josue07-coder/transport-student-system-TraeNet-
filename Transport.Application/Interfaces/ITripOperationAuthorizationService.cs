using Transport.Domain.Entities;

namespace Transport.Application.Interfaces
{
    public interface ITripOperationAuthorizationService
    {
        Task EnsureCanViewTripAsync(Trip trip);
        Task EnsureCanStartTripAsync(RouteAssignment assignment);
        Task EnsureCanEndTripAsync(Trip trip);
        Task EnsureCanManageTripAttendanceAsync(Trip trip);
        Task EnsureCanReportRouteDeviationAsync(Trip trip);
        Task EnsureCanUpdateLocationAsync(Trip trip);
        Task<bool> CanReportIncidentAsync(Trip? trip);
    }
}
