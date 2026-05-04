using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class RouteStop : BaseEntity
    {
        public Guid RouteId { get; private set; }   
        public Guid StopId { get; private set; }

        public int StopOrder { get; private set; }

        private RouteStop() { } // EF Core

        public RouteStop(Guid routeId, Guid stopId, int order)
        {
            if (routeId == Guid.Empty)
                throw new DomainException("La ruta es obligatorio");

            if (stopId == Guid.Empty)
                throw new DomainException("La parada es obligatorio");

            if (order <= 0)
                throw new DomainException("El orden debe ser mayor que 0");

            RouteId = routeId;
            StopId = stopId;
            StopOrder = order;
        }

        public void UpdateOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Order must be greater than zero");

            StopOrder = newOrder;
        }
    }
}