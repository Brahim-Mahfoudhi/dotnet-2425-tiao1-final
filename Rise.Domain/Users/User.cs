namespace Rise.Domain.Users;

/// <summary>
/// Represents a user entity in the system
/// </summary>
public class User : Entity
{
    #region Fields
    private string _firstName = default!;
    private string _lastName = default!;
    private string _email = default!;
    // private string _password = default!;
    private DateTime _birthDate;
    private Address _address = default!;
    private List<Role> _roles = [];
    private string _phoneNumber = default!;

    #endregion

    #region Constructors
    /// <summary>
    ///Private constructor for Entity Framework Core
    /// </summary>
    private User() { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the specified details.
    /// </summary>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="birthDate">The birth date of the user.</param>
    /// <param name="address">The address of the user.</param>
    /// <param name="phoneNumber">The phone number of the user.</param>
    public User(string firstName, string lastName, string email, DateTime birthDate, Address address, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        Address = address;
        PhoneNumber = phoneNumber;
    }
    #endregion


    #region Properties
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string FirstName
    {
        get => _firstName;
        set => _firstName = Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
    }
    
    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string LastName
    {
        get => _lastName;
        set => _lastName = Guard.Against.NullOrWhiteSpace(value, nameof(LastName));
    }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email
    {
        get => _email;
        set => _email = Guard.Against.NullOrWhiteSpace(value, nameof(Email));
    }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    // public string Password
    // {
    //     get => _password;
    //     set => _password = Guard.Against.NullOrWhiteSpace(value, nameof(Password));
    // }

    /// <summary>
    /// Gets or sets the birth date of the user.
    /// </summary>
    public DateTime BirthDate
    {
        get => _birthDate;
        set => _birthDate = Guard.Against.Default(value, nameof(BirthDate));
    }

    /// <summary>
    /// Gets or sets the address of the user.
    /// </summary>
    public Address Address
    {
        get => _address;
        set => _address = Guard.Against.Null(value, nameof(Address));
    }

    /// <summary>
    /// Gets the roles associated with the user.
    /// </summary>
    public IReadOnlyList<Role> Roles => _roles;

    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
    }
    #endregion


    #region Methods
    /// <summary>
    /// Adds a role to the user.
    /// </summary>
    /// <param name="role">The role to add.</param>
    public void AddRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Add(role);
    }

    /// <summary>
    /// Removes a role from the user.
    /// </summary>
    /// <param name="role">The role to remove.</param>
    public void RemoveRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Remove(role);
    }
    
    /// <summary>
    /// Marks the user as deleted (soft delete).
    /// </summary>
    public void SoftDelete()
    {
        IsDeleted = true;
    }

    /// <summary>
    /// Reactivates a previously deleted user.
    /// </summary>
    public void Activate()
    {
        IsDeleted = false;
    }
    #endregion
}

