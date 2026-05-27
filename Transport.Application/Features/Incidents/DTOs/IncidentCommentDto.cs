namespace Transport.Application.Features.Incidents.DTOs
{
    public class IncidentCommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
