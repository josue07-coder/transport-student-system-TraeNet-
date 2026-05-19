using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudentByCode
{
    public record GetStudentByCodeQuery(string Code) : IRequest<StudentDetailDto>;
}
