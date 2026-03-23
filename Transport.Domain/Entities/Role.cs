using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Role: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
