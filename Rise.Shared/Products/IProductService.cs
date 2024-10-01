using System.Threading;

namespace Rise.Shared.Products;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}