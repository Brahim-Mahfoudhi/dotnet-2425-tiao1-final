using Rise.Shared.Enums;
namespace Rise.Shared.Users;

public class RoleDto
{
    public int Id { get; set; }
    public RolesEnum Name { get; set; } = default!;
}

