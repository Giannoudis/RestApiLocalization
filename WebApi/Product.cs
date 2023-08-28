// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace RestApiLocalization.WebApi;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string>? NameLocalizations { get; set; }

    public decimal Price { get; set; }
    public Dictionary<string, decimal>? PriceLocalizations { get; set; }
}