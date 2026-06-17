using MediatR;

namespace Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentAbsent
{
    public class MarkStudentAbsentCommand : IRequest<Unit>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string? Notes { get; set; }
    }
}
