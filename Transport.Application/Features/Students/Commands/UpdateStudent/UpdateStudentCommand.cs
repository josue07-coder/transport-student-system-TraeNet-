using MediatR;

public class UpdateStudentCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Guid SchoolId { get; set; }
    public Guid GradeId { get; set; }
    public Guid GuardianId { get; set; }

    public string? PhotoUrl { get; set; }
}