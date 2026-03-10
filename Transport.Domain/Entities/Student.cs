using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        public Guid SchoolId { get; set; }

        public Guid GradeId { get; set; }

        public Guid GuardianId { get; set; }

        public School School { get; set; }

        public Grade Grade { get; set; }

        public Guardian Guardian { get; set; }

        public ICollection<StudentRouteAssignment> RouteAssignments { get; set; }
    }
}
