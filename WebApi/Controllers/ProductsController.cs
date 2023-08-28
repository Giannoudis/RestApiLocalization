using AutoMapper;
using RestApiLocalization.WebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace RestApiLocalization.WebApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    /// <summary>Get all products</summary>
    [HttpGet(Name = "GetProducts")]
    public IEnumerable<Product> GetProducts()
    {
        var products = new ProductService().GetProducts();
        return products;
    }

    /// <summary>Get localized products</summary>
    /// <param name="culture">The localization language (default: thread/invariant)</param>
    [HttpGet("dto", Name = "GetProductsDto")]
    public IEnumerable<ProductDto> GetProductsDto([FromQuery] string? culture = null)
    {
        // map products to dto
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDto>());
        var mapper = new Mapper(config);

        var products = new ProductService().GetProducts();
        var dataProducts = products.ConvertAll(
            // map object
            x => mapper.Map<ProductDto>(x)
                // map localizations
                .MapLocalizations(x, culture)).ToList();
        return dataProducts;
    }
}