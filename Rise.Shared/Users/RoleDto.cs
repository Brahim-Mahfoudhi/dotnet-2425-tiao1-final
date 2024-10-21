using Rise.Shared.Enums;
namespace Rise.Shared.Users;

/// <summary>
/// Data Transfer Object (DTO) representing a user.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public RolesEnum Name { get; set; } = default!;
}

