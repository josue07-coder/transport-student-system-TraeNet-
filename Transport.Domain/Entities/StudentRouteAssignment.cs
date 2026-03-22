using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class StudentRouteAssignment
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid RouteAssignmentId { get; set; }
        public RouteAssignment RouteAssignment { get; set; }

        public Guid StopId { get; set; }
        public Stop Stop { get; set; }
    }
}
