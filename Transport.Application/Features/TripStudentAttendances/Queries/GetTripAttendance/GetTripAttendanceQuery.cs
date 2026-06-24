using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetTripAttendance
{
    public record GetTripAttendanceQuery(Guid TripId) : IRequest<TripAttendanceSummaryDto>;
}
