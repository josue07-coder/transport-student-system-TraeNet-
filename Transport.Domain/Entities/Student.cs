using Transport.Domain.Common;
using Transport.Domain.Entities;
using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

public class Student : BaseEntity
{
    public StudentCode StudentCode { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid SchoolId { get; private set; }

    private readonly List<StudentRouteAssignment> _assignments = new();
    public IReadOnlyCollection<StudentRouteAssignment> Assignments => _assignments;

    public Student(string firstName, string lastName, StudentCode code, Guid schoolId)
    {
        SetName(firstName, lastName);

        StudentCode = code ?? throw new DomainException("Student code is required");

        if (schoolId == Guid.Empty)
            throw new DomainException("School is required");

        SchoolId = schoolId;
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required");

        FirstName = firstName;
        LastName = lastName;
    }

    public void AssignToRoute(Guid routeAssignmentId)
    {
        if (_assignments.Any(a => a.RouteAssignmentId == routeAssignmentId))
            throw new DomainException("Student already assigned to this route");

        _assignments.Add(new StudentRouteAssignment(Id, routeAssignmentId));
    }

    public void RemoveFromRoute(Guid routeAssignmentId)
    {
        var assignment = _assignments
            .FirstOrDefault(a => a.RouteAssignmentId == routeAssignmentId);

        if (assignment == null)
            throw new DomainException("Assignment not found");

        _assignments.Remove(assignment);
    }
}