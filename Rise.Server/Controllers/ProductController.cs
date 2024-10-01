using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Products;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductDto>> Get()
    {
        var products = await productService.GetProductsAsync();
        return products;
    }
}
