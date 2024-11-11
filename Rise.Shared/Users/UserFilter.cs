using Rise.Shared.Enums;

namespace Rise.Shared.Users;

public class UserFilter
{
    public RolesEnum? Role { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? RegisteredAfter { get; set; }
    public bool? IsDeleted { get; set; }
}