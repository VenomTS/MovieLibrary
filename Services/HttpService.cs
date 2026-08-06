using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using DTO.OFS.ResponseObject;
using Services.ResponseObject;

namespace Services;

public class HttpService : IHttpService
{
    private static readonly HttpClient Client = new();

    public static void SetBearerToken(string bearerToken)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }

    public static void SetBaseAddress(string baseIpAddress, string basePort)
    {
        Client.BaseAddress = new Uri($"http://{baseIpAddress}:{basePort}/api/");
    }
    
    public async Task<HttpResponseObject<TResponse>> PostJsonAsync<TRequest, TResponse>(string url, TRequest request, IDictionary<string, string>? headers = null)
    {
        var httpMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };
        
        if(headers != null)
            foreach(var header in headers)
                httpMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

        var response = await Client.SendAsync(httpMessage);
        
        // var response = await Client.PostAsJsonAsync(url, request);

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

    public async Task<HttpResponse> PostJsonAsync<TRequest>(string url, TRequest request)
    {
        var response = await Client.PostAsJsonAsync(url, request);

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

    public async Task<HttpResponseObject<string>> PostTextAsync(string url, string request)
    {
        var content = new StringContent(request, Encoding.UTF8, "text/plain");

        var response = await Client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = new HttpResponseObject<string>
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
                Content = null
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

        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return new HttpResponseObject<string>
            {
                IsSuccess = true,
                StatusCode = response.StatusCode,
                Content = responseContent
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to parse response from PostTextAsync: {e.Message}");
            throw;
        }
    }

    public async Task<HttpResponseObject<TResponse>> GetAsync<TResponse>(string url)
    {
        var response = await Client.GetAsync(url);

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
        var response = await Client.GetAsync(url);
        
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