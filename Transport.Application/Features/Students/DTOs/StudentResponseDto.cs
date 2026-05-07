

namespace Transport.Application.Features.Students.DTOs
{
    public class StudentResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public Guid SchoolId { get; set; }
        public Guid GradeId { get; set; }
        public Guid GuardianId { get; set; }
    }
}
