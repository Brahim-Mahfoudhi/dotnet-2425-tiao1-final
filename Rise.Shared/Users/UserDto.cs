namespace Rise.Shared.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public AddressDto Address { get; set; } = default!;
        public List<RoleDto> Roles { get; set; } = new();
        public string PhoneNumber { get; set; } = default!;

    }
}