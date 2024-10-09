namespace Rise.Domain.Users;

public class User : Entity
{
    #region Fields
    private string _firstName = default!;
    private string _lastName = default!;
    private string _email = default!;
    private string _password = default!;
    private DateTime _birthDate;
    private Address _address = default!;
    private List<Role> _roles = [];
    private string _phoneNumber = default!;

    #endregion

    #region Constructors
    protected User() { }
    public User(string firstName, string lastName, string email, string password, DateTime birthDate, Address address, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        BirthDate = birthDate;
        Address = address;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion


    #region Properties
    public string FirstName
    {
        get => _firstName;
        set => _firstName = Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
    }

    public string LastName
    {
        get => _lastName;
        set => _lastName = Guard.Against.NullOrWhiteSpace(value, nameof(LastName));
    }

    public string Email
    {
        get => _email;
        set => _email = Guard.Against.NullOrWhiteSpace(value, nameof(Email));
    }

    public string Password
    {
        get => _password;
        set => _password = Guard.Against.NullOrWhiteSpace(value, nameof(Password));
    }

    public DateTime BirthDate
    {
        get => _birthDate;
        set => _birthDate = Guard.Against.Default(value, nameof(BirthDate));
    }

    public Address Address
    {
        get => _address;
        set => _address = Guard.Against.Null(value, nameof(Address));
    }

    public IReadOnlyList<Role> Roles => _roles;

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
    }


    #endregion


    #region Methods
    public void AddRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Remove(role);
    }
    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void Activate()
    {
        IsDeleted = false;
    }
    #endregion
}

