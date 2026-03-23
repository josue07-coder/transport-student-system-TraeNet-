using Transport.Shared.Common;

namespace Transport.Domain.Entities
{
    public class RouteStop: BaseEntity
    {
        public Guid RouteId { get; set; }
        public Route Route { get; set; }

        public Guid StopId { get; set; }
        public Stop Stop { get; set; }

        public int StopOrder { get; set; }
    }
}
