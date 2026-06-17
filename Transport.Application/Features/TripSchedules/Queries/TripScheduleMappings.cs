using Transport.Application.Features.TripSchedules.DTOs;
using Transport.Domain.Entities;
using Transport.Domain.Enums;

namespace Transport.Application.Features.TripSchedules.Queries
{
    internal static class TripScheduleMappings
    {
        public static TripScheduleResponseDto ToResponseDto(TripSchedule schedule)
        {
            return new TripScheduleResponseDto
            {
                Id = schedule.Id,
                RouteAssignmentId = schedule.RouteAssignmentId,
                Direction = schedule.Direction,
                DirectionLabel = GetDirectionLabel(schedule.Direction),
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                IsActive = schedule.IsActive,
                ValidFrom = schedule.ValidFrom,
                ValidTo = schedule.ValidTo,
                DaysOfWeek = GetDaysOfWeek(schedule)
            };
        }

        public static TripScheduleDetailDto ToDetailDto(TripSchedule schedule)
        {
            var assignment = schedule.RouteAssignment;

            return new TripScheduleDetailDto
            {
                Id = schedule.Id,
                RouteAssignmentId = schedule.RouteAssignmentId,
                Direction = schedule.Direction,
                DirectionLabel = GetDirectionLabel(schedule.Direction),
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                IsActive = schedule.IsActive,
                ValidFrom = schedule.ValidFrom,
                ValidTo = schedule.ValidTo,
                DaysOfWeek = GetDaysOfWeek(schedule),
                RouteName = assignment?.Route?.Name,
                DriverName = assignment?.Driver == null ? null : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                VehiclePlate = assignment?.Vehicle?.PlateNumber
            };
        }

        private static string GetDirectionLabel(TripDirection direction)
        {
            return direction switch
            {
                TripDirection.ToSchool => "Entrada a la escuela",
                TripDirection.FromSchool => "Salida de la escuela",
                _ => direction.ToString()
            };
        }

        private static string GetDaysOfWeek(TripSchedule schedule)
        {
            var days = new List<string>();

            if (schedule.Monday) days.Add("Monday");
            if (schedule.Tuesday) days.Add("Tuesday");
            if (schedule.Wednesday) days.Add("Wednesday");
            if (schedule.Thursday) days.Add("Thursday");
            if (schedule.Friday) days.Add("Friday");
            if (schedule.Saturday) days.Add("Saturday");
            if (schedule.Sunday) days.Add("Sunday");

            return string.Join(", ", days);
        }
    }
}
