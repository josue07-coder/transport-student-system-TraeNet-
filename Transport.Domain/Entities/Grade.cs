using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class Grade : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid SchoolId { get; private set; }

        private Grade() { } // EF Core

        public Grade(string name, Guid schoolId, string? description = null)
        {
            SetName(name);

            if (schoolId == Guid.Empty)
                throw new DomainException("School is required");

            SchoolId = schoolId;
            Description = description ?? string.Empty;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Grade name is required");

            Name = name;
        }

        public void UpdateDescription(string? description)
        {
            Description = description ?? string.Empty;
        }
    }
}