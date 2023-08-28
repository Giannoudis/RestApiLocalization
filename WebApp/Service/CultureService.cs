using System.Text.Json;

namespace RestApiLocalization.WebApp.Service
{
    public class CultureService
    {
        private HttpClient HttpClient { get; }
        private readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public CultureService(HttpClient httpClient)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<string>> GetAvailableCulturesAsync()
        {
            var uri = "cultures";
            using var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new();
            }
            var cultures = JsonSerializer.Deserialize<List<string>>(json, serializerOptions);
            return cultures ?? new();
        }

        public async Task<List<CultureDescription>> GetAvailableCultureDescriptionsAsync()
        {
            var uri = "cultures/description";
            using var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new();
            }
            var cultures = JsonSerializer.Deserialize<List<CultureDescription>>(json, serializerOptions);
            return cultures ?? new();
        }
    }
}
