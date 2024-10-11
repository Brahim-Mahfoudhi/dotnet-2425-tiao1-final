using Rise.Domain.Products;

namespace Rise.Persistence;

/// <summary>
/// Responsible for seeding the database with initial data.
/// </summary>
public class Seeder
{
    private readonly ApplicationDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="Seeder"/> class with a specified <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="dbContext">The database context used for seeding.</param>
    public Seeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Seeds the database with initial data if it has not been seeded already.
    /// </summary>
    public void Seed()
    {
        if (!HasAlreadyBeenSeeded())
            SeedProducts();
        
    }

    /// <summary>
    /// Checks if the database has already been seeded with products.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the database already contains product entries; otherwise, <c>false</c>.
    /// </returns>
    private bool HasAlreadyBeenSeeded()
    {
        return dbContext.Products.Any();
    }

    /// <summary>
    /// Seeds the database with a range of product entities.
    /// </summary>
    private void SeedProducts()
    {
        var products = Enumerable.Range(1, 20)
                                 .Select(i => new Product { Name = $"Product {i}"})
                                 .ToList();

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}

