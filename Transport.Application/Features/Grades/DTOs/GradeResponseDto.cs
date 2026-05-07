namespace Transport.Application.Features.Grades.DTOs
{
    public class GradeResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SchoolId { get; set; }
    }
}