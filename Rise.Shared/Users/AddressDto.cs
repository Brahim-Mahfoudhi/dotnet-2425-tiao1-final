namespace Rise.Shared.Users
{
    public class AddressDto
    {
        public string Street { get; set; } = default!;
        public int HouseNumber { get; set; } = default!;
        public string? Bus { get; set; }
    }
}
