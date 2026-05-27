using MediatR;
using Transport.Application.Features.Students.Commands.CreateStudent;
using Transport.Application.Interfaces;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IStudentRepository _repo;
    private readonly IGuardianRepository _guardianRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IAuditService _auditService;

    public CreateStudentHandler(
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

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        if (!await _guardianRepository.IsActiveAsync(request.GuardianId))
            throw new DomainException("El acudiente está inactivo o no existe");

        if (!await _schoolRepository.IsActiveAsync(request.SchoolId))
            throw new DomainException("La escuela está inactiva o no existe");

        if (!await _gradeRepository.BelongsToSchoolAsync(request.GradeId, request.SchoolId))
            throw new DomainException("El grado no pertenece a la escuela seleccionada");

        StudentCode code;
        do
        {
            code = StudentCode.Create($"STU-{Guid.NewGuid().ToString()[..8]}");
        }
        while (await _repo.ExistsByCodeAsync(code.Value));

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

        await _auditService.LogAsync("Created", "Student", student.Id.ToString(), null, $"{{\"StudentCode\":\"{student.StudentCode.Value}\",\"GuardianId\":\"{student.GuardianId}\"}}");

        return student.Id;
    }
}
