using Newtonsoft.Json;
using System.Diagnostics;

namespace VanSalesJourneyPlan.Mobile.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object? data = null);
    Task<T?> PutAsync<T>(string endpoint, object? data = null);
    Task<bool> DeleteAsync(string endpoint);
    void SetToken(string token);
    void ClearToken();
    string? GetToken();
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private string? _token;
    private const string BaseUrl = "http://localhost:5249/api";

    public ApiClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    public void ClearToken()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public string? GetToken()
    {
        return _token;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GET Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            return default;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"POST Error: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            return default;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"PUT Error: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DELETE Error: {ex.Message}");
            return false;
        }
    }
}
