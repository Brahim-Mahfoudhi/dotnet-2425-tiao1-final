namespace Rise.Shared.Users;

/// <summary>
/// Data Transfer Object (DTO) representing a user with minimal info.
/// </summary>
public class UserDto
{
    public class GetUser
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; } = default!;
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; } = default!;
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = default!;
        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new();

        public bool IsDeleted { get; set; } = default!;
    }
        
    public class GetUserDetails : GetUser
    {
        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public AddressDto.GetAdress Address { get; set; } = default!;
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; } = default!;
    }

    public class CreateUser : GetUser
    {
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; } = default!;
        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public AddressDto.GetAdress Address { get; set; } = default!;
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; } = default!;
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
}
