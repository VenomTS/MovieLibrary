using System.Net.Http.Headers;
using System.Net.Http.Json;
using DTO.OFS;
using DTO.OFS.Configuration;
using DTO.OFS.Fiscalization.InvoiceIssue;
using DTO.OFS.Fiscalization.Status;
using Services.ResponseObject;

namespace Services;

// OVA KLASA MORA BITI DODANA KAO SINGLETON
public class OFSService
{
    private readonly HttpClient _httpClient = new();

    private string _baseUrl = string.Empty;
    private string _key = string.Empty;
    private string _pin = string.Empty;

    public void Initialize(string ip, string port, string key, string pin)
    {
        _baseUrl = $"http://{ip}:{port}/api";
        _key = key;
        _pin = pin;
    }

    public async Task<ApiResponse> VerifyAvailabilityAsync()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/attention");
        message = SetAuthorizationHeader(message);
        
        var response = await _httpClient.SendAsync(message);

        return await GetApiResponseAsync(response);
    }
    
    public async Task<ApiResponseObject<StatusResponse>> VerifyStatusAsync()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/status");
        message = SetAuthorizationHeader(message);
        
        var response = await _httpClient.SendAsync(message);
        
        return await GetApiResponseAsync<StatusResponse>(response);
    }

    public async Task<ApiResponseObject<string>> GetSettingsAsync()
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/settings");
        message = SetAuthorizationHeader(message);
        
        var response = await _httpClient.SendAsync(message);
        
        // U content ce se vratiti string ovaj citav
        var apiResponse = new ApiResponseObject<string>()
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
        };

        try
        {
            apiResponse.Content = await response.Content.ReadAsStringAsync();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to parse settings as a string. Error: {ex.Message}");
        }

        return apiResponse;

        // return await GetApiResponseAsync<ConfigurationResponse>(response);
    }

    public async Task<ApiResponse> SetSecurityParametersAsync(SecurityParametersRequest request)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/settings")
        {
            Content = JsonContent.Create(request)
        };
        message = SetAuthorizationHeader(message);
        
        var response = await _httpClient.SendAsync(message);

        return await GetApiResponseAsync(response);
    }

    public async Task<ApiResponse> SetPrinterSettingsAsync(PrinterConfigurationRequest request)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/settings")
        {
            Content = JsonContent.Create(request)
        };
        message = SetAuthorizationHeader(message);

        var response = await _httpClient.SendAsync(message);

        return await GetApiResponseAsync(response);
    }

    public async Task<ApiResponseObject<InvoiceIssueResponse>> IssueInvoice(InvoiceIssueRequest request,
        InvoiceHeaders? headers = null)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/invoices")
        {
            Content = JsonContent.Create(request)
        };
        message = SetAuthorizationHeader(message);

        if(headers?.RequestId != null)
            message.Headers.TryAddWithoutValidation("RequestId", headers.RequestId);
        if(headers?.XTeronSerialNumber != null)
            message.Headers.TryAddWithoutValidation("X-Teron-SerialNumber", headers.XTeronSerialNumber);
        
        var response = await _httpClient.SendAsync(message);
        
        return await GetApiResponseAsync<InvoiceIssueResponse>(response);
    }

    private static async Task<ApiResponse> GetApiResponseAsync(HttpResponseMessage response)
    {
        return new ApiResponse
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Message = await GetMessageResponseAsync(response)
        };
    }

    private static async Task<ApiResponseObject<T>> GetApiResponseAsync<T>(HttpResponseMessage response)
    {
        return new ApiResponseObject<T>
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Message = await GetMessageResponseAsync(response),
            Content = await GetContentResponseAsync<T>(response)
        };
    }

    private static async Task<T?> GetContentResponseAsync<T>(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadFromJsonAsync<T>();
            return content;
        }
        catch
        {
            return default;
        }
        
    }

    private static async Task<string> GetMessageResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var msg = await response.Content.ReadAsStringAsync();
            return msg;
        }
        catch
        {
            return string.Empty;
        }
    }

    private HttpRequestMessage SetAuthorizationHeader(HttpRequestMessage message)
    {
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _key);
        return message;
    }
}