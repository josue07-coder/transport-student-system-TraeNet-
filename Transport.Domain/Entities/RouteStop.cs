using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class RouteStop
    {
        public Guid Id { get; set; }

        public Guid RouteId { get; set; }
        public Route Route { get; set; }

        public Guid StopId { get; set; }
        public Stop Stop { get; set; }

        public int StopOrder { get; set; }
    }
}
