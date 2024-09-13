using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Asegúrate de importar esto

public class GoogleCloudVisionService
{
    private readonly IConfiguration _configuration; // Agrega esto
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public GoogleCloudVisionService(HttpClient httpClient, IConfiguration configuration) // Ajusta el constructor
    {
        _httpClient = httpClient;
        _configuration = configuration; // Asigna el valor
        _apiKey = _configuration["ApiKeys:GoogleCloudVision"]; // Mueve esto aquí
    }

    public async Task<string> DetectTextFromImageAsync(string imageBase64)
    {
        var url = $"https://vision.googleapis.com/v1/images:annotate?key={_apiKey}";

        var requestBody = new
        {
            requests = new[]
            {
                new
                {
                    image = new { content = imageBase64 },
                    features = new[]
                    {
                        new { type = "TEXT_DETECTION" }
                    }
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return jsonResponse;
    }
}