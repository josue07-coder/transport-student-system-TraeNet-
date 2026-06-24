using MediatR;

namespace Transport.Application.Features.TripStudentAttendances.Commands.CloseTripAttendance
{
    public record CloseTripAttendanceCommand(Guid TripId) : IRequest;
}
