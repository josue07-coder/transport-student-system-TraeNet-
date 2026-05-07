using MediatR;

using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudentById
{
    public record GetStudentByIdQuery(Guid Id) : IRequest<StudentResponseDto>;
}
