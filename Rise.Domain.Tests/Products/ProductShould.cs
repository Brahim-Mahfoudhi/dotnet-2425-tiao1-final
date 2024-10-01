using Rise.Domain.Products;
using Shouldly;

namespace Rise.Domain.Tests.Products;
/// <summary>
/// Example Domain Tests using xUnit and Shouldly
/// https://xunit.net
/// https://docs.shouldly.org
/// </summary>
public class ProductShould
{
    [Fact]
    public void BeCreated()
    {
        Product p = new() { Name = "iPhone 16" };

        p.Name.ShouldBe("iPhone 16");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            Product product = new() { Name = name! };
        };
        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("")]
    public void NotBeChangedToHaveAnInvalidName(string? name)
    {
        Action act = () =>
        {
            Product p = new() { Name = "iPhone 16" };
            p.Name = name!;
        };

        act.ShouldThrow<ArgumentException>();
    }
}
