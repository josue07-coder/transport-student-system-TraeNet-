using MediatR;

namespace Transport.Application.Features.Students.Commands.CreateStudent
{
    public record CreateStudentCommand(
        string FirstName,
        string LastName,
        string Code,
        Guid SchoolId
    ) : IRequest<Guid>;
}
