namespace Rise.Shared.Users
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a user.
    /// </summary>
    public class UserDto
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
        public AddressDto Address { get; set; } = default!;
        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new();
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; } = default!;

    }
}