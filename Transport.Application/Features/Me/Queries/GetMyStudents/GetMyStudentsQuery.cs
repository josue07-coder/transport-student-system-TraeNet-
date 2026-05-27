using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Me.Queries.GetMyStudents
{
    public record GetMyStudentsQuery : IRequest<List<StudentResponseDto>>;
}
