using MediatR;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;

public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Unit>
{
    private readonly IStudentRepository _repo;
    private readonly IGuardianRepository _guardianRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IAuditService _auditService;

    public UpdateStudentHandler(
        IStudentRepository repo,
        IGuardianRepository guardianRepository,
        ISchoolRepository schoolRepository,
        IGradeRepository gradeRepository,
        IAuditService auditService)
    {
        _repo = repo;
        _guardianRepository = guardianRepository;
        _schoolRepository = schoolRepository;
        _gradeRepository = gradeRepository;
        _auditService = auditService;
    }

    public async Task<Unit> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _repo.GetByIdAsync(request.Id);

        if (student == null)
            throw new DomainException("Student not found");

        var oldValues = $"{{\"FirstName\":\"{student.FirstName}\",\"LastName\":\"{student.LastName}\",\"SchoolId\":\"{student.SchoolId}\",\"GradeId\":\"{student.GradeId}\",\"GuardianId\":\"{student.GuardianId}\"}}";

        if (!await _guardianRepository.IsActiveAsync(request.GuardianId))
            throw new DomainException("El acudiente está inactivo o no existe");

        if (!await _schoolRepository.IsActiveAsync(request.SchoolId))
            throw new DomainException("La escuela está inactiva o no existe");

        if (!await _gradeRepository.BelongsToSchoolAsync(request.GradeId, request.SchoolId))
            throw new DomainException("El grado no pertenece a la escuela seleccionada");

        student.SetName(request.FirstName, request.LastName);
        student.UpdateSchool(request.SchoolId);
        student.UpdateGrade(request.GradeId);
        student.UpdateGuardian(request.GuardianId);
        student.UpdatePhoto(request.PhotoUrl);

        await _repo.SaveChangesAsync();

        var newValues = $"{{\"FirstName\":\"{student.FirstName}\",\"LastName\":\"{student.LastName}\",\"SchoolId\":\"{student.SchoolId}\",\"GradeId\":\"{student.GradeId}\",\"GuardianId\":\"{student.GuardianId}\"}}";
        await _auditService.LogAsync("Updated", "Student", student.Id.ToString(), oldValues, newValues);

        return Unit.Value;
    }
}
