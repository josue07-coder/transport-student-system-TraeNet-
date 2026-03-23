

namespace Transport.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; }

        public Guid SectorId { get; }

        public Address(string street, Guid sectorId)
        {
            Street = street;
            SectorId = sectorId;
        }
    }
}
