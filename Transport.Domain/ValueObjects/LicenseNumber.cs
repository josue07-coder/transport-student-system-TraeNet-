using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.ValueObjects
{
    public class LicenseNumber
    {
        public string Value { get; }

        public LicenseNumber(string value)
        {
            Value = value;
        }
    }
}
