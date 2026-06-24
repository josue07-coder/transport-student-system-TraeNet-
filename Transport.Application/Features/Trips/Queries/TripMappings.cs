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
                TripScheduleId = trip.TripScheduleId,
                Direction = trip.Direction,
                OperationDate = trip.OperationDate,
                ScheduledDepartureTime = trip.ScheduledDepartureTime,
                ScheduledArrivalTime = trip.ScheduledArrivalTime,
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                Status = trip.Status,
                CancellationReason = trip.CancellationReason,
                NonOperationReason = trip.NonOperationReason,
                NonOperationNotes = trip.NonOperationNotes,
                DelayMinutes = trip.DelayMinutes,
                IsLate = trip.IsLate,
                StartedEarly = trip.StartedEarly,
                EarlyStartReason = trip.EarlyStartReason,
                PunctualityStatus = trip.PunctualityStatus
            };
        }

        public static TripDetailDto ToDetailDto(Trip trip)
        {
            var assignment = trip.RouteAssignment;

            return new TripDetailDto
            {
                Id = trip.Id,
                RouteAssignmentId = trip.RouteAssignmentId,
                TripScheduleId = trip.TripScheduleId,
                Direction = trip.Direction,
                OperationDate = trip.OperationDate,
                ScheduledDepartureTime = trip.ScheduledDepartureTime,
                ScheduledArrivalTime = trip.ScheduledArrivalTime,
                RouteId = assignment?.RouteId,
                RouteName = assignment?.Route?.Name,
                DriverId = assignment?.DriverId,
                DriverName = assignment?.Driver == null ? null : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                VehicleId = assignment?.VehicleId,
                PlateNumber = assignment?.Vehicle?.PlateNumber,
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                Status = trip.Status,
                CancellationReason = trip.CancellationReason,
                NonOperationReason = trip.NonOperationReason,
                NonOperationNotes = trip.NonOperationNotes,
                DelayMinutes = trip.DelayMinutes,
                IsLate = trip.IsLate,
                StartedEarly = trip.StartedEarly,
                EarlyStartReason = trip.EarlyStartReason,
                PunctualityStatus = trip.PunctualityStatus,
                HasRouteDeviation = trip.RouteDeviations.Any(),
                RouteDeviationCount = trip.RouteDeviations.Count
            };
        }
    }
}
