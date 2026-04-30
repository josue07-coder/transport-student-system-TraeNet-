using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudents
{
    public record GetStudentsQuery() : IRequest<List<StudentDto>>;
}
