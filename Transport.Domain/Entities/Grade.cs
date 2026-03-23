using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Grade: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Student> Students { get; set; }

    }
}
