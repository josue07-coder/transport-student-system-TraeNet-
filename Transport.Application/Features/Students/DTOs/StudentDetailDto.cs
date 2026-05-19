namespace Transport.Application.Features.Students.DTOs
{
    public class StudentDetailDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public Guid SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public Guid GradeId { get; set; }
        public string? GradeName { get; set; }
        public Guid GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
