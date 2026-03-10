using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.ValueObjects
{
    public class StudentCode
    {
        public string Value { get; }

        public StudentCode(string value)
        {
            Value = value;
        }
    }
}
