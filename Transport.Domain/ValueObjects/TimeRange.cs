using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class TimeRange : ValueObject
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public TimeRange(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new DomainException("End time must be greater than start time");

            var duration = end - start;

            if (duration < TimeSpan.FromMinutes(10))
                throw new DomainException("La duración mínima de la ruta es de 10 minutos");

            if (duration > TimeSpan.FromHours(4))
                throw new DomainException("La duración máxima de la ruta es de 4 horas");

            Start = start;
            End = end;
        }

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= Start && dateTime <= End;
        }

        public bool Overlaps(TimeRange other)
        {
            return Start < other.End && End > other.Start;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}
