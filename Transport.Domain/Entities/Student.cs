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
    public string? PhotoUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<StudentRouteAssignment> _assignments = new();
    public IReadOnlyCollection<StudentRouteAssignment> Assignments => _assignments;

    private Student() { }

    public Student(string firstName, string lastName, StudentCode code, Guid schoolId)
    {
        SetName(firstName, lastName);

        StudentCode = code ?? throw new DomainException("El codigo del estudiante es obligatorio");

        if (schoolId == Guid.Empty)
            throw new DomainException("La escuela es obligatorio");

        SchoolId = schoolId;
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("El nombre es obligatorio");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Appellidos obligatorios");

        FirstName = firstName;
        LastName = lastName;
    }

    public void AssignToRoute(Guid routeAssignmentId)
    {
        if (_assignments.Any(a => a.RouteAssignmentId == routeAssignmentId))
            throw new DomainException("El estudiante ya esta asignado a esta ruta");

        _assignments.Add(new StudentRouteAssignment(Id, routeAssignmentId));
    }

    public void RemoveFromRoute(Guid routeAssignmentId)
    {
        var assignment = _assignments
            .FirstOrDefault(a => a.RouteAssignmentId == routeAssignmentId);

        if (assignment == null)
            throw new DomainException("Asignacion no encontrada");

        _assignments.Remove(assignment);
    }
    public void UpdatePhoto(string? photoUrl)
    {
        PhotoUrl = photoUrl;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}