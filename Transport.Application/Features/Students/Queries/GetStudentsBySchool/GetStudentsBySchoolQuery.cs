using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudentsBySchool
{
    public record GetStudentsBySchoolQuery(Guid SchoolId) : IRequest<List<StudentResponseDto>>;
}
