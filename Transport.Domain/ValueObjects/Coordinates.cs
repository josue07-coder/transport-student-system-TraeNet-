using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class Coordinates : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }

        private Coordinates(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
        private Coordinates() { }

        public static Coordinates Create(double lat, double lng)
        {
            if (lat < -90 || lat > 90)
                throw new DomainException("Invalid latitude");

            if (lng < -180 || lng > 180)
                throw new DomainException("Invalid longitude");

            return new Coordinates(lat, lng);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
