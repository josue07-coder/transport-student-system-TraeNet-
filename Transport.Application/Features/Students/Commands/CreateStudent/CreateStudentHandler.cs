using MediatR;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Interfaces;
using Transport.Domain.ValueObjects;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IStudentRepository _repo;

    public CreateStudentHandler(IStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        //  Generar código automáticamente
        var code = StudentCode.Create($"STU-{Guid.NewGuid().ToString().Substring(0, 8)}");

        var student = new Student(
            request.FirstName,
            request.LastName,
            code,
            request.SchoolId,
            request.GradeId,
            request.GuardianId
        );

        await _repo.AddAsync(student);
        await _repo.SaveChangesAsync();

        return student.Id;
    }
}