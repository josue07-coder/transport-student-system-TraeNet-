using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudentsByGrade
{
    public record GetStudentsByGradeQuery(Guid GradeId) : IRequest<List<StudentResponseDto>>;
}
