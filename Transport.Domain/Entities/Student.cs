using Transport.Domain.Enums;
using Transport.Domain.ValueObjects;
using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Student: BaseEntity
    {
        public StudentCode StudentCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? PhotoUrl { get; set; }

        public Guid GuardianId { get; set; }
        public Guardian Guardian { get; set; }

        public Address Address { get; set; }

        public Guid SchoolId { get; set; }
        public School School { get; set; }

        public Guid GradeId { get; set; }
        public Grade Grade { get; set; }

        public ICollection<StudentRouteAssignment> StudentRouteAssignments { get; set; } = new List<StudentRouteAssignment>();
    }
}
