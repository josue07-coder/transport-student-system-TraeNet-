using Transport.Application.Features.Trips.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.Trips.Queries
{
    internal static class TripMappings
    {
        public static TripResponseDto ToResponseDto(Trip trip)
        {
            return new TripResponseDto
            {
                Id = trip.Id,
                RouteAssignmentId = trip.RouteAssignmentId,
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                Status = trip.Status
            };
        }

        public static TripDetailDto ToDetailDto(Trip trip)
        {
            var assignment = trip.RouteAssignment;

            return new TripDetailDto
            {
                Id = trip.Id,
                RouteAssignmentId = trip.RouteAssignmentId,
                RouteId = assignment?.RouteId,
                RouteName = assignment?.Route?.Name,
                DriverId = assignment?.DriverId,
                DriverName = assignment?.Driver == null ? null : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                VehicleId = assignment?.VehicleId,
                PlateNumber = assignment?.Vehicle?.PlateNumber,
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                Status = trip.Status
            };
        }
    }
}
