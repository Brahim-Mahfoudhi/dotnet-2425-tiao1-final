using Rise.Shared.Enums;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Street is required.")]
        public StreetEnum Street { get; set; } = StreetEnum.AFRIKALAAN;
        /// <summary>
        /// Gets or sets the house number of the address.
        /// </summary>
        [NotNullOrEmpty]
        [RegularExpression(@"^\d+\s?[A-Za-z]?$", ErrorMessage = "House number must be a number or a number followed by a letter.")]
        public string? HouseNumber { get; set; } = null;
        /// <summary>
        /// Gets or sets the optional bus number for the address.
        /// </summary>
        public string? Bus { get; set; }
    }

    public class CreateAddress : AddressDto.GetAdress
    {
        // Default constructor
        public CreateAddress()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAddress"/> class.
        /// </summary>
        /// <param name="street">The street name for the address.</param>
        /// <param name="houseNumber">The house number for the address.</param>
        /// <param name="bus">The optional bus number for the address.</param>
        public CreateAddress(StreetEnum street, string houseNumber, string? bus = null)
        {
            Street = street;
            HouseNumber = houseNumber;
            Bus = bus;
        }

    }

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

