

namespace Transport.Application.Features.Students.DTOs
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public Guid SchoolId { get; set; }
    }
}
