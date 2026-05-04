

namespace Transport.Application.Features.Students.DTOs
{
    public class StudentDetailDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }

        public List<Guid> RouteAssignments { get; set; }
    }
}
