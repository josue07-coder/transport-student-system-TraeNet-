namespace Transport.Application.Features.TransportAssistants.DTOs
{
    public class TransportAssistantResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}
