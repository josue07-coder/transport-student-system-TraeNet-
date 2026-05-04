using Transport.Domain.Exceptions;
using Transport.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }

    private Address() { } // 🔥 EF

    private Address(string street, string city)
    {
        Street = street;
        City = city;
    }

    public static Address Create(string street, string city)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("La calle es obligatorio");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("La ciudad es ibligatorio");

        return new Address(street, city);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
    }
}