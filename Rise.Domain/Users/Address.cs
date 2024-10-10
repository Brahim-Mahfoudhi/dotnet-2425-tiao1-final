namespace Rise.Domain.Users;

public class Address : Entity
{
    private StreetEnum _street = default!;
    private int _houseNumber = default!;
    private string? _bus = default!;

    //Private constructor for EF Core
    private Address()
    {
    }

    public Address(string street, int houseNumber)
    {
        Street = street;
        HouseNumber = houseNumber;
    }

    public Address(string street, int houseNumber, string? bus = null)
    {
        Street = street;
        HouseNumber = houseNumber;
        Bus = bus;
    }

    public string Street
    {
        get => _street.GetStreetName();
        set => _street = Guard.Against.Null(StreetEnumExtensions.GetStreetEnum(value));
    }

    public int HouseNumber
    {
        get => _houseNumber;
        set => _houseNumber = Guard.Against.NegativeOrZero(value);
    }

    public string? Bus
    {
        get => _bus;
        set => _bus = value;
    }
}

