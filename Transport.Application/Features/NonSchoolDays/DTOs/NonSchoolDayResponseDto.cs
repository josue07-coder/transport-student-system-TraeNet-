using Transport.Domain.Enums;

namespace Transport.Application.Features.NonSchoolDays.DTOs
{
    public class NonSchoolDayResponseDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public Guid? SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public NonSchoolDayReason ReasonType { get; set; }
        public string ReasonTypeLabel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
