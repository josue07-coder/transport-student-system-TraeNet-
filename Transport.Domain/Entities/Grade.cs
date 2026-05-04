using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Grade : BaseEntity
    {
        public string Name { get; private set; }

        public Guid SchoolId { get; private set; }

        private readonly List<Student> _students = new();
        public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

        private Grade() { } // EF

        public Grade(string name, Guid schoolId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Grade name is required");

            if (schoolId == Guid.Empty)
                throw new DomainException("School is required");

            Name = name;
            SchoolId = schoolId;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Grade name is required");

            Name = name;
        }
    }
}