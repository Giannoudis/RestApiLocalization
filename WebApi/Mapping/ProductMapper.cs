using Riok.Mapperly.Abstractions;

namespace RestApiLocalization.WebApi.Mapping;

/// <summary>Compile-time generated mapper for <see cref="Product"/> to <see cref="ProductDto"/>.</summary>
[Mapper]
public partial class ProductMapper
{
    /// <summary>Map a product to its DTO (base values only; localizations are applied separately).</summary>
    [MapperIgnoreSource(nameof(Product.NameLocalizations))]
    [MapperIgnoreSource(nameof(Product.PriceLocalizations))]
    public partial ProductDto ToDto(Product source);
}
