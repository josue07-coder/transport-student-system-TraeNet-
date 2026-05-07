using MediatR;
using Transport.Application.Interfaces;

public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Unit>
{
    private readonly IStudentRepository _repo;

    public UpdateStudentHandler(IStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _repo.GetByIdAsync(request.Id);

        if (student == null)
            throw new Exception("Student not found");

        student.SetName(request.FirstName, request.LastName);

        //  actualizar relaciones
        student.UpdateSchool(request.SchoolId);
        student.UpdateGrade(request.GradeId);
        student.UpdateGuardian(request.GuardianId);

        student.UpdatePhoto(request.PhotoUrl);

        await _repo.SaveChangesAsync();

        return Unit.Value;
    }
}