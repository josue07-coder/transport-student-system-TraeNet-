using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Domain.Entities;

namespace Transport.Application.Features.TripStudentAttendances
{
    internal static class TripStudentAttendanceMappings
    {
        public static TripStudentAttendanceDto ToDto(TripStudentAttendance attendance)
        {
            return new TripStudentAttendanceDto
            {
                TripId = attendance.TripId,
                StudentId = attendance.StudentId,
                StudentName = attendance.StudentNameSnapshot,
                StudentCode = attendance.StudentCodeSnapshot,
                GuardianId = attendance.GuardianIdSnapshot,
                GuardianName = attendance.GuardianNameSnapshot,
                Status = attendance.Status,
                BoardedAt = attendance.BoardedAt,
                DroppedOffAt = attendance.DroppedOffAt,
                Notes = attendance.Notes
            };
        }
    }
}
