using MediatR;
using Transport.Application.Features.Grades.DTOs;

namespace Transport.Application.Features.Grades.Queries.GetGradesBySchool
{
    public record GetGradesBySchoolQuery(Guid SchoolId) : IRequest<List<GradeResponseDto>>;
}
