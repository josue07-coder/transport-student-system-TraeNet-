using Transport.Application.Features.RouteAssignments.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.RouteAssignments.Queries
{
    internal static class RouteAssignmentMappings
    {
        public static RouteAssignmentResponseDto ToResponseDto(RouteAssignment assignment)
        {
            return new RouteAssignmentResponseDto
            {
                Id = assignment.Id,
                RouteId = assignment.RouteId,
                RouteName = assignment.Route?.Name ?? string.Empty,
                DriverId = assignment.DriverId,
                DriverName = assignment.Driver == null ? string.Empty : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                LicenseNumber = assignment.Driver?.LicenseNumber?.Value ?? string.Empty,
                VehicleId = assignment.VehicleId,
                PlateNumber = assignment.Vehicle?.PlateNumber ?? string.Empty,
                VehicleCapacity = assignment.VehicleCapacity,
                AssignedStudentsCount = assignment.Students.Count,
                TripsCount = assignment.Trips.Count
            };
        }

        public static RouteAssignmentDetailDto ToDetailDto(RouteAssignment assignment)
        {
            return new RouteAssignmentDetailDto
            {
                Id = assignment.Id,
                RouteId = assignment.RouteId,
                RouteName = assignment.Route?.Name ?? string.Empty,
                DriverId = assignment.DriverId,
                DriverName = assignment.Driver == null ? string.Empty : $"{assignment.Driver.FirstName} {assignment.Driver.LastName}",
                LicenseNumber = assignment.Driver?.LicenseNumber?.Value ?? string.Empty,
                VehicleId = assignment.VehicleId,
                PlateNumber = assignment.Vehicle?.PlateNumber ?? string.Empty,
                VehicleCapacity = assignment.VehicleCapacity,
                TripsCount = assignment.Trips.Count,
                Students = assignment.Students
                    .Select(s => s.Student)
                    .Where(student => student != null)
                    .Select(student => new AssignedStudentDto
                    {
                        Id = student.Id,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        FullName = $"{student.FirstName} {student.LastName}",
                        StudentCode = student.StudentCode.Value
                    })
                    .ToList()
            };
        }
    }
}
