using Transport.Domain.Enums;

namespace Transport.Application.Features.Trips.DTOs
{
    public class TripResponseDto
    {
        public Guid Id { get; set; }
        public Guid RouteAssignmentId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TripStatus Status { get; set; }
    }
}
