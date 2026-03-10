using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport.Domain.ValueObjects
{
    public class TimeRange
    {
        public TimeSpan Start { get; }

        public TimeSpan End { get; }

        public TimeRange(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }
    }
}
