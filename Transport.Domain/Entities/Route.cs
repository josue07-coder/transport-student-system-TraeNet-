using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid SchoolId { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public TimeSpan ReturnTime { get; set; }

        public ICollection<RouteStop> Stops { get; set; }
    }
}
