namespace Rise.Domain.Users;

public class Address : Entity
{

    private StreetEnum street = default!;
    private int houseNumber = default!;
    private string? bus = default!;
    public required string Street
    {
        get => street.GetStreetName();
        set => street = StreetEnumExtensions.GetStreetEnum(value);
    }
    public required int HouseNumber { get => houseNumber; set => houseNumber = Guard.Against.NegativeOrZero(value); }
    public string? Bus { get => bus; set => bus = value; }
}

