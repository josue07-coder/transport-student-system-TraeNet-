using MediatR;
using Transport.Application.Features.Grades.DTOs;

namespace Transport.Application.Features.Grades.Queries.GetGradeById
{
    public record GetGradeByIdQuery(Guid Id) : IRequest<GradeResponseDto>;
}
