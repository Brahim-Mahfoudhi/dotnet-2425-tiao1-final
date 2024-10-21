namespace Rise.Shared.Users
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an address.
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// Gets or sets the street name of the address.
        /// </summary>
        public string Street { get; set; } = default!;
        /// <summary>
        /// Gets or sets the house number of the address.
        /// </summary>
        public int HouseNumber { get; set; } = default!;
        /// <summary>
        /// Gets or sets the optional bus number for the address.
        /// </summary>
        public string? Bus { get; set; }
    }
}
