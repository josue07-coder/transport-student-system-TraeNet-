using MediatR;
using Transport.Application.Features.Students.Commands.RemoveStudentFromRoute;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

public class RemoveStudentFromRouteHandler : IRequestHandler<RemoveStudentFromRouteCommand, Unit>
{
    private readonly IStudentRepository _studentRepository;

    public RemoveStudentFromRouteHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Unit> Handle(RemoveStudentFromRouteCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId)
            ?? throw new DomainException("Estudiante no encontrado");

        student.RemoveFromRoute(request.RouteAssignmentId);

        await _studentRepository.SaveChangesAsync();

        return Unit.Value;
    }
}