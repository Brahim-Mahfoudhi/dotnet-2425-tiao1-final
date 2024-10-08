namespace Rise.Domain.Users
{
    public class User : Entity
    {
        private string _firstName = default!;
        private string _lastName = default!;
        private string _email = default!;
        private string _password = default!;
        private DateTime _birthDate;
        private Address _address = default!;
        private List<Role> _roles = [];
        private string _phoneNumber = default!;

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
            get => _firstName;
            private set => _firstName = Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
        }

        public string LastName
        {
            get => _lastName;
            private set => _lastName = Guard.Against.NullOrWhiteSpace(value, nameof(LastName));
        }

        public string Email
        {
            get => _email;
            private set => _email = Guard.Against.NullOrWhiteSpace(value, nameof(Email));
        }

        public string Password
        {
            get => _password;
            private set => _password = Guard.Against.NullOrWhiteSpace(value, nameof(Password));
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            private set => _birthDate = Guard.Against.Default(value, nameof(BirthDate));
        }

        public Address Address
        {
            get => _address;
            set => _address = Guard.Against.Null(value, nameof(Address));
        }

        public IReadOnlyList<Role> Roles => _roles;

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

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
        }
    }
}
