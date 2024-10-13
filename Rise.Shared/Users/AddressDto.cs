using Rise.Shared.Enums;

namespace Rise.Shared.Users;

/// <summary>
/// Data Transfer Object (DTO) representing an address.
/// </summary>
public class AddressDto
{
    public class GetAdress
    {
        /// <summary>
        /// Gets or sets the street name of the address.
        /// </summary>
        public StreetEnum Street { get; set; } = StreetEnum.AFRIKALAAN;
        /// <summary>
        /// Gets or sets the house number of the address.
        /// </summary>
        public string? HouseNumber { get; set; }
        /// <summary>
        /// Gets or sets the optional bus number for the address.
        /// </summary>
        public string? Bus { get; set; }
    }
    public class CreateAddress : AddressDto.GetAdress;

    public class UpdateAddress
    {
        /// <summary>
        /// Gets or sets the street name of the address.
        /// </summary>
        public StreetEnum? Street { get; set; } = null;
        /// <summary>
        /// Gets or sets the house number of the address.
        /// </summary>
        public int? HouseNumber { get; set; } = null;
        /// <summary>
        /// Gets or sets the optional bus number for the address.
        /// </summary>
        public string? Bus { get; set; }
    }
}

