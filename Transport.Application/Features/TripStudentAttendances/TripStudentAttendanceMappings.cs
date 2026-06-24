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
                StudentNameSnapshot = attendance.StudentNameSnapshot,
                StudentCodeSnapshot = attendance.StudentCodeSnapshot,
                GuardianId = attendance.GuardianIdSnapshot,
                GuardianName = attendance.GuardianNameSnapshot,
                GuardianNameSnapshot = attendance.GuardianNameSnapshot,
                SchoolNameSnapshot = attendance.Student?.School?.Name,
                Status = attendance.Status,
                BoardedAt = attendance.BoardedAt,
                DroppedOffAt = attendance.DroppedOffAt,
                Notes = attendance.Notes,
                IsExpectedPassenger = attendance.IsExpectedPassenger,
                ExceptionReason = attendance.ExceptionReason,
                RegisteredByUserId = attendance.RegisteredByUserId,
                RegisteredByName = attendance.RegisteredByUser?.Name,
                RegisteredAt = attendance.RegisteredAt,
                MarkedByUserId = attendance.MarkedByUserId,
                MarkedByName = attendance.MarkedByUser?.Name,
                MarkedAt = attendance.MarkedAt,
                AbsenceNotifiedAt = attendance.AbsenceNotifiedAt,
                BoardedNotificationSentAt = attendance.BoardedNotificationSentAt,
                AttendanceSource = attendance.AttendanceSource
            };
        }
    }
}
