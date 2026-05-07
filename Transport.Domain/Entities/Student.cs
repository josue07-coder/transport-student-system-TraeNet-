using Transport.Domain.Common;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

public class Student : BaseEntity, IActivatable
{
    public StudentCode StudentCode { get; private set; }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public Guid SchoolId { get; private set; }
    public Guid GradeId { get; private set; }
    public Guid GuardianId { get; private set; }

    public string? PhotoUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    
    public Grade Grade { get; private set; }
    public Guardian Guardian { get; private set; }

    private readonly List<StudentRouteAssignment> _assignments = new();
    public IReadOnlyCollection<StudentRouteAssignment> Assignments => _assignments.AsReadOnly();

    private Student() { } // EF Core

    public Student(
    string firstName,
    string lastName,
    StudentCode code,
    Guid schoolId,
    Guid gradeId,
    Guid guardianId)
    {
        SetName(firstName, lastName);

        StudentCode = code ?? throw new DomainException("El codigo del estudiante es obligatorio");

        if (schoolId == Guid.Empty)
            throw new DomainException("La escuela es obligatoria");

        if (gradeId == Guid.Empty)
            throw new DomainException("El grado es obligatorio");

        if (guardianId == Guid.Empty)
            throw new DomainException("El guardian es obligatorio");

        SchoolId = schoolId;
        GradeId = gradeId;
        GuardianId = guardianId;
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("El nombre es obligatorio");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Los apellidos son obligatorios");

        FirstName = firstName;
        LastName = lastName;
    }

    public void AssignToRoute(Guid routeAssignmentId)
    {
        if (_assignments.Any(a => a.RouteAssignmentId == routeAssignmentId))
            throw new DomainException("El estudiante ya está asignado a esta ruta");

        _assignments.Add(new StudentRouteAssignment(Id, routeAssignmentId));
    }

    public void RemoveFromRoute(Guid routeAssignmentId)
    {
        var assignment = _assignments
            .FirstOrDefault(a => a.RouteAssignmentId == routeAssignmentId);

        if (assignment == null)
            throw new DomainException("Asignación no encontrada");

        _assignments.Remove(assignment);
    }

    public void UpdatePhoto(string? photoUrl)
    {
        PhotoUrl = photoUrl;
    }
    public void UpdateSchool(Guid schoolId)
    {
        if (schoolId == Guid.Empty)
            throw new DomainException("School is required");

        SchoolId = schoolId;
    }

    public void UpdateGrade(Guid gradeId)
    {
        if (gradeId == Guid.Empty)
            throw new DomainException("Grade is required");

        GradeId = gradeId;
    }

    public void UpdateGuardian(Guid guardianId)
    {
        if (guardianId == Guid.Empty)
            throw new DomainException("Guardian is required");

        GuardianId = guardianId;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}