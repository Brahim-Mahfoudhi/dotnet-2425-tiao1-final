using System.Collections.Immutable;

namespace Rise.Shared.Users;
/// <summary>
/// Data Transfer Object (DTO) representing a user with minimal info.
/// </summary>
public class UserDto
{
    public record UserBase
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

    public record UserDetails
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
    
    public record RegistrationUser
    {
        public int Id;
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public bool IsDeleted { get; init; }
        public string Password { get; init; }
        public string PhoneNumber { get; init; }
        public ImmutableList<RoleDto>? Roles { get; init; } = ImmutableList<RoleDto>.Empty;
        public DateTime BirthDate { get; init; } = DateTime.Now;
        public AddressDto.CreateAddress Address { get; init; } = new AddressDto.CreateAddress();

    }
    
    public class UpdateUser
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string? FirstName { get; set; } = null;
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string? LastName { get; set; } = null;
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; } = null;
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string? Password { get; set; } = null;
        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public AddressDto.UpdateAddress? Address { get; set; } = null;
        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<RoleDto>? Roles { get; set; } = null;
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string? PhoneNumber { get; set; } = null;
    }

    public record CreateUserAuth0
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; } = null;
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string? FirstName { get; set; } = null;
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string? LastName { get; set; } = null;
        /// <summary>
        /// Gets or sets the connection type to API, default is "Password-Username-Authentication"
        /// </summary>
        public string? Connection { get; set; } = "Username-Password-Authentication";
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string? Password { get; set; } = null;
        
    }

    public class UserTable
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required bool Blocked { get; set; }
    }
}
