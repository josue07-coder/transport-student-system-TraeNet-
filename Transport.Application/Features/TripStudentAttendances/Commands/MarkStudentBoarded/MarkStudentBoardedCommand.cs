using MediatR;

namespace Transport.Application.Features.TripStudentAttendances.Commands.MarkStudentBoarded
{
    public class MarkStudentBoardedCommand : IRequest<Unit>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
    }
}
