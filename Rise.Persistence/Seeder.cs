using Rise.Domain.Products;

namespace Rise.Persistence;

public class Seeder
{
    private readonly ApplicationDbContext dbContext;

    public Seeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Seed()
    {
        if (HasAlreadyBeenSeeded())
            return;

        SeedProducts();
    }

    private bool HasAlreadyBeenSeeded()
    {
        return dbContext.Products.Any();
    }

    private void SeedProducts()
    {
        var products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}"})
                                 .ToList();

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}

