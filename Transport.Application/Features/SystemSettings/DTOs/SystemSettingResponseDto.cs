namespace Transport.Application.Features.SystemSettings.DTOs
{
    public class SystemSettingResponseDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsEditable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
