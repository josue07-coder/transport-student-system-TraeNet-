using MediatR;

namespace Transport.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid SchoolId { get; set; }
        public Guid GradeId { get; set; }
        public Guid GuardianId { get; set; }
    }
}
