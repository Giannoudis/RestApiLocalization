using Microsoft.AspNetCore.Components;
using RestApiLocalization.WebApp.Model;
using RestApiLocalization.WebApp.Service;

namespace RestApiLocalization.WebApp.Pages;

public partial class Products
{
    [Inject] public ProductService? ProductService { get; set; }
    private List<Product>? products;
    private List<ProductDto>? dataProducts;
    private string? ErrorMessage { get; set; }

    private string? GetLocalizations(Product product, string propertyName)
    {
        var localizations = product.GetLocalizations(propertyName);
        if (!localizations.Any())
        {
            return null;
        }
        return string.Join(",", localizations.Select(kv => kv.Key + "=" + kv.Value).ToArray());
    }

    private async Task SetupProducts()
    {
        try
        {
            if (ProductService != null)
            {
                products = await ProductService.GetProductsAsync();
                dataProducts = await ProductService.GetDataProductsAsync();
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.GetBaseException().Message;
        }
    }
    protected override async Task OnInitializedAsync()
    {
        await SetupProducts();
        await base.OnInitializedAsync();
    }
}