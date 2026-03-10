using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public string PlateNumber { get; set; }

        public string Brand { get; set; }

        public int Capacity { get; set; }

        public ICollection<RouteAssignment> RouteAssignments { get; set; }
    }
}
