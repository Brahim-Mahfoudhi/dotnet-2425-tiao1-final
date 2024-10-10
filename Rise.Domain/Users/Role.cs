namespace Rise.Domain.Users;

public class Role : Entity
{
    private RolesEnum _name;

    public RolesEnum Name
    {
        get => _name;
        set => _name = Guard.Against.EnumOutOfRange(value);
    }

    //Private constructor for EF Core
    private Role()
    {
    }

    public Role(RolesEnum name = RolesEnum.User)
    {
        Name = name;
    }
}
