using System.Collections.Immutable;

namespace Rise.Shared.Users;
/// <summary>
/// Data Transfer Object (DTO) representing a user with minimal info.
/// Used records for the DTO's itself, because they are immutable by design
/// Using sealed records gives a performance gain
/// </summary>
public class UserDto
{
    public sealed record UserBase
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public bool IsDeleted { get; init; }
        public ImmutableList<RoleDto> Roles { get; init; } = ImmutableList<RoleDto>.Empty;

        // Constructor to initialize everything
        public UserBase(int id, string firstName, string lastName, string email, 
            ImmutableList<RoleDto>? roles = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Roles = roles ?? ImmutableList<RoleDto>.Empty;
        }
    }
    /// <summary>
    /// DTO for writing User to DB
    /// </summary>
    public sealed record UserDb
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public AddressDto.GetAdress Address { get; init; }
        public DateTime BirthDate { get; init; } = DateTime.Now;
        
        public UserDb(string firstName, string lastName, string email, string phoneNumber,
            AddressDto.GetAdress address , DateTime? birthDate = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            BirthDate = birthDate ?? DateTime.Now;
        }
    }

    /// <summary>
    /// DTO for showing users details in table on Users page
    /// </summary>
    public sealed record UserDetails
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public AddressDto.GetAdress Address { get; init; }
        public ImmutableList<RoleDto> Roles { get; init; } = ImmutableList<RoleDto>.Empty;
        public DateTime BirthDate { get; init; } = DateTime.Now;
        
        public UserDetails(int id, string firstName, string lastName, string email, 
            AddressDto.GetAdress address ,ImmutableList<RoleDto>? roles = null, DateTime? birthDate = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Address = address;
            Roles = roles ?? ImmutableList<RoleDto>.Empty;
            BirthDate = birthDate ?? DateTime.Now;
        }
    }
    
    /// <summary>
    /// DTO used for registrationform
    /// </summary>
    public sealed record RegistrationUser
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string PhoneNumber { get; init; }
        public ImmutableList<RoleDto>? Roles { get; init; } = ImmutableList<RoleDto>.Empty;
        public DateTime BirthDate { get; init; } = DateTime.Now;
        public AddressDto.CreateAddress Address { get; init; } = new AddressDto.CreateAddress();
        
        public RegistrationUser(string firstName, string lastName, string email, string phoneNumber,
            AddressDto.CreateAddress address , ImmutableList<RoleDto>? roles = null, DateTime? birthDate = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Roles = roles ?? ImmutableList<RoleDto>.Empty;
            BirthDate = birthDate ?? DateTime.Now;
        }
    }
    
    /// <summary>
    /// DTO used to update a User in the DB
    /// Almost identical to RegistrationUser, but Id is now also included
    /// </summary>
    public sealed record UpdateUser
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public DateTime? BirthDate { get; init; } = DateTime.Now;
        public AddressDto.CreateAddress Address { get; init; } = new AddressDto.CreateAddress();
        public ImmutableList<RoleDto>? Roles { get; init; } = ImmutableList<RoleDto>.Empty;
        public string PhoneNumber { get; init; }
        
        public UpdateUser(int id, string firstName, string lastName, string email,string passsword, string phoneNumber,
            AddressDto.CreateAddress address , ImmutableList<RoleDto>? roles = null, DateTime? birthDate = null)
        {
            Id= id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = passsword;
            PhoneNumber = phoneNumber;
            Address = address;
            Roles = roles ?? ImmutableList<RoleDto>.Empty;
            BirthDate = birthDate ?? DateTime.Now;
        }
    }

    /// <summary>
    /// DTO used to register new user in Auth0
    /// </summary>
    public sealed record CreateUserAuth0(
        string email,
        string firstName,
        string lastName,
        string password,
        string connection = "Username-Password-Authentication");

    /// <summary>
    /// DTO used for showing users in the table on the AuthUsers page
    /// </summary>
    public sealed record UserTable(string Email, string FirstName, string LastName, bool Blocked);

}
