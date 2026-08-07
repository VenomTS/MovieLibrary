using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using DTO.OFS.ResponseObject;
using Services.ResponseObject;

namespace Services;

public class HttpService : IHttpService
{
    private static readonly HttpClient Client = new();
    private Uri _baseIpAddress;

    public void SetBearerToken(string bearerToken)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }

    public void SetBaseAddress(string baseIpAddress, string basePort)
    {
        _baseIpAddress = new Uri($"http://{baseIpAddress}:{basePort}/api/");
    }
    
    public async Task<HttpResponseObject<TResponse>> PostJsonAsync<TRequest, TResponse>(string url, TRequest request, IDictionary<string, string>? headers = null)
    {
        var finalUrl = new Uri($"{_baseIpAddress}{url}");
        var httpMessage = new HttpRequestMessage(HttpMethod.Post, finalUrl)
        {
            Content = JsonContent.Create(request)
        };
        
        if(headers != null)
            foreach(var header in headers)
                httpMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

        var response = await Client.SendAsync(httpMessage);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = new HttpResponseObject<TResponse>
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
                Content = default
            };

            try
            {
                var message = await response.Content.ReadAsStringAsync();
                responseContent.Message = message;
            }
            catch (Exception)
            {
                // Ignored
            }
            return responseContent;
        }
        
        var content = await response.Content.ReadFromJsonAsync<TResponse>();
        return new HttpResponseObject<TResponse>
        {
            IsSuccess = true,
            StatusCode = response.StatusCode,
            Content = content
        };
    }

    public async Task<HttpResponse> PostJsonAsync<TRequest>(string url, TRequest request, IDictionary<string, string>? headers = null)
    {
        var finalUrl = new Uri($"{_baseIpAddress}{url}");
        var httpMessage = new HttpRequestMessage(HttpMethod.Post, finalUrl)
        {
            Content = JsonContent.Create(request)
        };
        
        if(headers != null)
            foreach(var header in headers)
                httpMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
        
        var response = await Client.SendAsync(httpMessage);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = new HttpResponse
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
            };

            try
            {
                var message = await response.Content.ReadAsStringAsync();
                responseContent.Message = message;
            }
            catch (Exception)
            {
                // Ignored
            }
            return responseContent;
        }

        var responseObject = new HttpResponse
        {
            IsSuccess = true,
            StatusCode = response.StatusCode,
        };

        try
        {
            var content = await response.Content.ReadAsStringAsync();
            responseObject.Message = content;
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to parse message from PostJsonAsync");
            // ignored
        }
        
        return responseObject;
    }

    public async Task<HttpResponseObject<TResponse>> GetAsync<TResponse>(string url)
    {
        var finalUrl = new Uri($"{_baseIpAddress}{url}");
        var response = await Client.GetAsync(finalUrl);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = new HttpResponseObject<TResponse>
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
            };

            try
            {
                var message = await response.Content.ReadAsStringAsync();
                responseContent.Message = message;
            }
            catch (Exception)
            {
                // Ignored
            }
            return responseContent;
        }
        
        var content = await response.Content.ReadFromJsonAsync<TResponse>();
        return new HttpResponseObject<TResponse>
        {
            IsSuccess = true,
            StatusCode = response.StatusCode,
            Content = content
        };
    }

    public async Task<HttpResponse> GetAsync(string url)
    {
        var finalUrl = new Uri($"{_baseIpAddress}{url}");
        var response = await Client.GetAsync(finalUrl);
        
        var responseContent = new HttpResponse
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
        };

        try
        {
            var message = await response.Content.ReadAsStringAsync();
            responseContent.Message = message;
        }
        catch (Exception)
        {
            //
        }
        return responseContent;
    }
}