using MediatR;
using Transport.Application.Features.TripStudentAttendances.DTOs;

namespace Transport.Application.Features.TripStudentAttendances.Queries.SearchAttendanceStudents
{
    public record SearchAttendanceStudentsQuery(Guid TripId, string Query) : IRequest<List<AttendanceStudentSearchResultDto>>;
}
