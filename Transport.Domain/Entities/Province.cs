using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class Province
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public String PostalCode { get; set; }
        public ICollection<Municipality> Municipalities { get; set; }
    }
}
