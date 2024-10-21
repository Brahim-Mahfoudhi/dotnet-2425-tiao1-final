namespace Rise.Domain.Users;
using Rise.Shared.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a user's address in the system.
/// </summary>
public class Address : Entity
{
    private string _userId; // Foreign key referencing User
    private User _user;
    private StreetEnum _street = default!;
    private string _houseNumber = default!;
    private string? _bus = default!;

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private Address()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class with the specified street and house number.
    /// </summary>
    /// <param name="street">The street of the address.</param>
    /// <param name="houseNumber">The house number of the address.</param> 
    public Address(string street, string houseNumber)
    {
        Street = street;
        HouseNumber = houseNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class with the specified street, house number, and optional bus number.
    /// </summary>
    /// <param name="street">The street of the address.</param>
    /// <param name="houseNumber">The house number of the address.</param>
    /// <param name="bus">The optional bus number of the address.</param>
    public Address(string street, string houseNumber, string? bus = null)
    {
        Street = street;
        HouseNumber = houseNumber;
        Bus = bus;
    }

    public string UserId
    {
        get => _userId;
        set => _userId = Guard.Against.Null(value);
    }

    /// <summary>
    /// Gets or sets the user associated with this address.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the user is <c>null</c>.
    /// </exception>
    public User User
    {
        get => _user;
        set => _user = Guard.Against.Null(value);
    }

    /// <summary>
    /// Gets or sets the street name of the address.
    /// </summary>
    /// <remarks>
    /// The street name is derived from the <see cref="StreetEnum"/> enumeration.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the street is <c>null</c>.
    /// </exception>
    [Required(ErrorMessage = "Street is required.")]
    public string Street
    {
        get => _street.GetStreetName();
        set => _street = Guard.Against.Null(StreetEnumExtensions.GetStreetEnum(value));
    }

    /// <summary>
    /// Gets or sets the house number of the address.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the house number is less than or equal to zero.
    /// </exception>
    [NotNullOrEmpty]
    [RegularExpression(@"^\d+\s?[A-Za-z]?$", ErrorMessage = "House number must be a number or a number followed by a letter.")]
    public string HouseNumber
    {
        get => _houseNumber;
        set => _houseNumber = Guard.Against.NullOrWhiteSpace(value, nameof(HouseNumber));
    }

    /// <summary>
    /// Gets or sets the optional bus number of the address.
    /// </summary>
    public string? Bus
    {
        get => _bus;
        set => _bus = value;
    }
}

