

using Transport.Domain.Exceptions;

namespace Transport.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }

        private Address(string street, string city)
        {
            Street = street;
            City = city;
        }

        public static Address Create(string street, string city)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street is required");

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City is required");

            return new Address(street, city);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
        }
    }
}
