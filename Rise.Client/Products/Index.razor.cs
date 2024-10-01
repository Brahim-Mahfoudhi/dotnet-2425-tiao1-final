using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;

namespace Rise.Client.Products;

public partial class Index
{
    private IEnumerable<ProductDto>? products;

    [Inject] public required IProductService ProductService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }
}

