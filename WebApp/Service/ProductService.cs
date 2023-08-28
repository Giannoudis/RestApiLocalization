using System.Text.Json;
using RestApiLocalization.WebApp.Model;

namespace RestApiLocalization.WebApp.Service
{
    public class ProductService
    {
        private HttpClient HttpClient { get; }
        private readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ProductService(HttpClient httpClient)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }


        public async Task<List<Product>> GetProductsAsync()
        {
            var uri = "products";
            using var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new();
            }
            var products = JsonSerializer.Deserialize<List<Product>>(json, serializerOptions);
            return products ?? new();
        }

        public async Task<List<ProductDto>> GetDataProductsAsync()
        {
            var uri = $"products/dto?culture={Thread.CurrentThread.CurrentUICulture.Name}";
            using var dtoResponse = await HttpClient.GetAsync(uri);
            dtoResponse.EnsureSuccessStatusCode();
            var json = await dtoResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new();
            }
            var dataProducts = JsonSerializer.Deserialize<List<ProductDto>>(json, serializerOptions);
            return dataProducts ?? new();
        }
    }
}
