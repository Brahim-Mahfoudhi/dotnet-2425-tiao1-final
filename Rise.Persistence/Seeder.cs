using Rise.Domain.Products;
using Rise.Domain.Users;

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
        if (!ProductsHasAlreadyBeenSeeded())
            SeedProducts();
        if (!UsersHasAlreadyBeenSeeded())
            SeedUsers();
    }

    /// <summary>
    /// Checks if the database has already been seeded with products.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the database already contains product entries; otherwise, <c>false</c>.
    /// </returns>
    private bool ProductsHasAlreadyBeenSeeded()
    {
        return dbContext.Products.Any();
    }
    
    /// <summary>
    /// Checks if the database has already been seeded with users.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the database already contains user entries; otherwise, <c>false</c>.
    /// </returns>
    private bool UsersHasAlreadyBeenSeeded()
    {
        return dbContext.Users.Any();
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

    /// <summary>
    /// Seeds the database with 2 user entities.
    /// </summary>
    private void SeedUsers()
    {
        dbContext.Users.Add(new User("Lorenz", "Debie", "lorenzdebie@gmail.com", "123456", new DateTime(1980, 01, 01), new Address("Brusselsesteenweg", 5), "+32478457845"));
    }
}

