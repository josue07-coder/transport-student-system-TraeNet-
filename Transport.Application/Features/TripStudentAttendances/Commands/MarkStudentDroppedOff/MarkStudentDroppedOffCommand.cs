using MediatR;

namespace Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentDroppedOff
{
    public class MarkStudentDroppedOffCommand : IRequest<Unit>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
    }
}
