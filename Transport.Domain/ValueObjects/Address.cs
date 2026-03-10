

namespace Transport.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; }

        public string Sector { get; }

        public string City { get; }

        public Address(string street, string sector, string city)
        {
            Street = street;
            Sector = sector;
            City = city;
        }
    }
}
