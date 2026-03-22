using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.Entities
{
    public class DriverLicense
    {
        public Guid Id { get; set; }
        public int  LincenseNumber { get; set; }
        public String CategoryCode { get; set; }
        public DateTime ExpirationDate { get; set; }

        public Guid DriverId { get; set; }
        public Driver Driver { get; set; }

        


    }
}
