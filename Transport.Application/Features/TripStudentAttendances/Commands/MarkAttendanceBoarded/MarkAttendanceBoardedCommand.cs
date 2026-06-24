using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Commands.MarkAttendanceBoarded
{
    public class MarkAttendanceBoardedCommand : IRequest<TripStudentAttendanceDto>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string? ExceptionReason { get; set; }
    }
}
