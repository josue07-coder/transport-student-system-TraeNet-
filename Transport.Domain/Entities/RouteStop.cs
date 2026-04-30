using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class RouteStop : BaseEntity
    {
        public Guid StopId { get; private set; }
        public int Order { get; private set; }

        private RouteStop() { } // EF Core

        public RouteStop(Guid stopId, int order)
        {
            if (stopId == Guid.Empty)
                throw new DomainException("Stop is required");

            if (order <= 0)
                throw new DomainException("Order must be greater than zero");

            StopId = stopId;
            Order = order;
        }

        public void UpdateOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Order must be greater than zero");

            Order = newOrder;
        }
    }
}