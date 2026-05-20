using Transport.Domain.Enums;

namespace Transport.Application.Features.Routes.DTOs
{
    public class RouteResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SchoolId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RouteStatus Status { get; set; }
        public int StopCount { get; set; }
    }
}
