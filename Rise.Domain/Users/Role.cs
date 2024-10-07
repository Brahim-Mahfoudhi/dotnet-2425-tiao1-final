namespace Rise.Domain.Users;

public class Role : Entity
{
    private RolesEnum name = RolesEnum.User;

    public required RolesEnum Name
    {
        get => name;
        set => name = Guard.Against.Null(value);
    }
}