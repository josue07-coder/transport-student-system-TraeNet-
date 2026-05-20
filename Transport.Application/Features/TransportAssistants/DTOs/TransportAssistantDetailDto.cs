using Transport.Domain.Enums;

namespace Transport.Application.Features.TransportAssistants.DTOs
{
    public class TransportAssistantDetailDto
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
