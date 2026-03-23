using Transport.Domain.Enums;
using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class Trip: BaseEntity
    {
        public Guid RouteAssignmentId { get; set; }
        public RouteAssignment RouteAssignment { get; set; }

        public DateTime TripDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public double? StartLatitude { get; set; }
        public double? StartLongitude { get; set; }

        public double? EndLatitude { get; set; }
        public double? EndLongitude { get; set; }

        public int? TotalStudentsAssigned { get; set; }
        public int? TotalStudentsPickedUp { get; set; }

       public TripStatus TripStatus { get; set; }
    }
}
