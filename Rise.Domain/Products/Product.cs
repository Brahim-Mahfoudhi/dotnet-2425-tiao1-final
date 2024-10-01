namespace Rise.Domain.Products;
public class Product : Entity
{
    private string name = default!;

    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }
}

