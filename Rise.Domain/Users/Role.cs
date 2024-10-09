namespace Rise.Domain.Users;

public class Role : Entity
{
    private RolesEnum _name = RolesEnum.User;

    public RolesEnum Name
    {
        get => _name;
        set => _name = Guard.Against.EnumOutOfRange(value);
    }

    public Role(RolesEnum name)
    {
        Name = name;
    }
}
