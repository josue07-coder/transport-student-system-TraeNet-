using MediatR;

namespace Transport.Application.Features.TripStudentAttendances.Commands.UpdateTripStudentNotes
{
    public class UpdateTripStudentNotesCommand : IRequest<Unit>
    {
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
