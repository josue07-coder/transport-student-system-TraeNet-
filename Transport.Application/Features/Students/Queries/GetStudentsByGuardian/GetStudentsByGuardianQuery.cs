using MediatR;
using Transport.Application.Features.Students.DTOs;

namespace Transport.Application.Features.Students.Queries.GetStudentsByGuardian
{
    public record GetStudentsByGuardianQuery(Guid GuardianId) : IRequest<List<StudentResponseDto>>;
}
