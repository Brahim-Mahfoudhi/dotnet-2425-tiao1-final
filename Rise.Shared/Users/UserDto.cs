using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

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

        public string Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public bool IsDeleted { get; init; }
        public ImmutableList<RoleDto> Roles { get; init; } = ImmutableList<RoleDto>.Empty;
//         /// <summary>
//         /// Gets or sets the unique identifier of the user.
//         /// </summary>
//         public string Id { get; set; }
//         /// <summary>
//         /// Gets or sets the first name of the user.
//         /// </summary>
//         [Required(ErrorMessage = "First name is required.")]
//         public string FirstName { get; set; } = default!;
//         /// <summary>
//         /// Gets or sets the last name of the user.
//         /// </summary>
//         [Required(ErrorMessage = "Last name is required.")]
//         public string LastName { get; set; } = default!;
//         /// <summary>
//         /// Gets or sets the email address of the user.
//         /// </summary>
//         [Required(ErrorMessage = "Email is required.")]
//         [EmailAddress(ErrorMessage = "Invalid email address format.")]
//         public string Email { get; set; } = default!;
//         /// <summary>
//         /// Gets or sets the list of roles assigned to the user.
//         /// </summary>
//         public List<RoleDto> Roles { get; set; } = new();

//         public bool IsDeleted { get; set; } = default!;
    }


        // Constructor to initialize everything
        public UserBase(string id, string firstName, string lastName, string email, 
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
    /// DTO for showing users details in table on Users page
    /// </summary>
    public sealed record UserDetails
    {
        public string Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public AddressDto.GetAdress Address { get; init; }
        public ImmutableList<RoleDto> Roles { get; init; } = ImmutableList<RoleDto>.Empty;
        public DateTime BirthDate { get; init; } = DateTime.Now;
        
        public UserDetails(string id, string firstName, string lastName, string email, 
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
//         /// <summary>
//         /// Gets or sets the password of the user.
//         /// </summary>
//         [Required(ErrorMessage = "Password is required.")]
//         public string Password { get; set; } = default!;
//         /// <summary>
//         /// Gets or sets the birth date of the user.
//         /// </summary>
//         [DateInThePast(ErrorMessage = "Birthdate must be a date in the past or today.")]
//         public DateTime BirthDate { get; set; } = DateTime.Now;
//         /// <summary>
//         /// Gets or sets the address of the user.
//         /// </summary>
//         public AddressDto.CreateAddress Address { get; set; } = new();
//         /// <summary>
//         /// Gets or sets the phone number of the user.
//         /// </summary>
//         [Required(ErrorMessage = "Phone number is required.")]
//         // [BelgianPhoneNumber]
//         public string PhoneNumber { get; set; } = default!;
    }

    /// <summary>
    /// DTO used for registrationform
    /// </summary>
    public sealed record RegistrationUser(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string? Password,
        string? Id,
        AddressDto.GetAdress? Address,
        DateTime? BirthDate = null)
    {
        public DateTime? BirthDate { get; init; } = BirthDate ?? DateTime.UtcNow;
    };
    
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
    /// DTO used for showing users in the table on the AuthUsers page
    /// </summary>
    public sealed record Auth0User(string Email, string FirstName, string LastName, bool Blocked);


}
