using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class Municipality
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public ICollection<Sector> Sectors { get; set; }
    }
}
