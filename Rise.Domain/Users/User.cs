namespace Rise.Domain.Users
{
    public class User : Entity
    {
        private string firstName = default!;
        private string lastName = default!;
        private string email = default!;
        private string password = default!;
        private DateTime birthDate;
        private Address address = default!;
        private List<Role> roles = [];
        private string phoneNumber = default!;

        public User(string firstName, string lastName, string email, string password, DateTime birthDate, Address address, string phoneNumber)
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            BirthDate = birthDate;
            Address = address;
            PhoneNumber = phoneNumber;
        }

        public string FirstName
        {
            get => firstName;
            private set => firstName = Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
        }

        public string LastName
        {
            get => lastName;
            private set => lastName = Guard.Against.NullOrWhiteSpace(value, nameof(LastName));
        }

        public string Email
        {
            get => email;
            private set => email = Guard.Against.NullOrWhiteSpace(value, nameof(Email));
        }

        public string Password
        {
            get => password;
            private set => password = Guard.Against.NullOrWhiteSpace(value, nameof(Password));
        }

        public DateTime BirthDate
        {
            get => birthDate;
            private set => birthDate = Guard.Against.Default(value, nameof(BirthDate));
        }

        public Address Address
        {
            get => address;
            private set => address = Guard.Against.Null(value, nameof(Address));
        }

        public IReadOnlyList<Role> Roles => roles;

        public void AddRole(Role role)
        {
            Guard.Against.Null(role, nameof(role));
            roles.Add(role);
        }

        public void RemoveRole(Role role)
        {
            Guard.Against.Null(role, nameof(role));
            roles.Remove(role);
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set => phoneNumber = Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
        }
    }
}