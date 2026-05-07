using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudents
{
    public class GetAllStudentsQuery : IRequest<List<StudentResponseDto>>
    {
    }
}
