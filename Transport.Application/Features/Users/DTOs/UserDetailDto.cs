namespace Transport.Application.Features.Users.DTOs
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public bool IsActive { get; set; }
        public string? ProfileImageUrl { get; set; }
        public Guid? GuardianId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? TransportAssistantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
