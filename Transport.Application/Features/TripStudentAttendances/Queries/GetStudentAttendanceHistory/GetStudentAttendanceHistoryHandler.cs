using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetStudentAttendanceHistory
{
    public class GetStudentAttendanceHistoryHandler : IRequestHandler<GetStudentAttendanceHistoryQuery, List<StudentAttendanceHistoryDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITripStudentAttendanceRepository _attendanceRepository;
        private readonly IVisibilityService _visibilityService;

        public GetStudentAttendanceHistoryHandler(
            IStudentRepository studentRepository,
            ITripStudentAttendanceRepository attendanceRepository,
            IVisibilityService visibilityService)
        {
            _studentRepository = studentRepository;
            _attendanceRepository = attendanceRepository;
            _visibilityService = visibilityService;
        }

        public async Task<List<StudentAttendanceHistoryDto>> Handle(GetStudentAttendanceHistoryQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DomainException("Estudiante no encontrado");

            var attendances = await _attendanceRepository.GetByStudentAsync(request.StudentId);
            var user = await _visibilityService.GetCurrentUserAsync();
            var access = FilterVisibleAttendances(user, student, attendances);

            if (!access.CanAccess)
                throw new DomainException("No tiene permiso para consultar el historial de asistencia de este estudiante");

            return access.Attendances.Select(attendance => new StudentAttendanceHistoryDto
            {
                TripId = attendance.TripId,
                RouteAssignmentId = attendance.Trip.RouteAssignmentId,
                RouteName = attendance.Trip.RouteAssignment.Route?.Name,
                OperationDate = attendance.Trip.OperationDate,
                TripStartTime = attendance.Trip.StartTime,
                Status = attendance.Status,
                BoardedAt = attendance.BoardedAt,
                DroppedOffAt = attendance.DroppedOffAt,
                IsExpectedPassenger = attendance.IsExpectedPassenger,
                ExceptionReason = attendance.ExceptionReason,
                AttendanceSource = attendance.AttendanceSource
            }).ToList();
        }

        private static (bool CanAccess, List<TripStudentAttendance> Attendances) FilterVisibleAttendances(
            User user,
            Student student,
            IEnumerable<TripStudentAttendance> attendances)
        {
            var attendanceList = attendances.ToList();

            if (IsRole(user, "Admin") || IsRole(user, "Supervisor"))
                return (true, attendanceList);

            if (IsRole(user, "Guardian"))
            {
                return user.GuardianId.HasValue && student.GuardianId == user.GuardianId.Value
                    ? (true, attendanceList)
                    : (false, new List<TripStudentAttendance>());
            }

            if (IsRole(user, "Driver"))
            {
                var visible = user.DriverId.HasValue
                    ? attendanceList.Where(attendance =>
                        attendance.Trip.RouteAssignment?.DriverId == user.DriverId.Value).ToList()
                    : new List<TripStudentAttendance>();

                return (visible.Count > 0, visible);
            }

            if (IsRole(user, "TransportAssistant"))
            {
                var visible = user.TransportAssistantId.HasValue
                    ? attendanceList.Where(attendance =>
                        attendance.Trip.RouteAssignment?.TransportAssistantId == user.TransportAssistantId.Value).ToList()
                    : new List<TripStudentAttendance>();

                return (visible.Count > 0, visible);
            }

            return (false, new List<TripStudentAttendance>());
        }

        private static bool IsRole(User user, string role)
        {
            return string.Equals(user.Role?.Name, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
