using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Queries.GetStudentAttendanceHistory
{
    public record GetStudentAttendanceHistoryQuery(Guid StudentId) : IRequest<List<StudentAttendanceHistoryDto>>;
}
