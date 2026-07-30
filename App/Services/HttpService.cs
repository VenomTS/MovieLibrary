using App.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using App.APIResponses;

namespace App.Services
{
    public class HttpService : IHttpService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        private const string DefaultAddress = "http://localhost:5126/api/v1/";
        private readonly HttpClient _client;

        public HttpService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(DefaultAddress)
            };
        }

        public void SetJwt(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<APIResponse<TResponse>> GetAsync<TResponse>(string url)
        {
            var response = await _client.GetAsync(url);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                };

            // Response failed
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<ErrorResponse>(_options);

                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = content
                };
            }
            
            var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
            return new APIResponse<TResponse>
            {
                Status = response.StatusCode,
                Content = result,
                ErrorResponse = null,
            };
        }

        public async Task<APIResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _client.PostAsJsonAsync(url, data, _options);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                };

            // Response failed
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<ErrorResponse>(_options);

                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = content
                };
            }

            try
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = result,
                    ErrorResponse = null,
                };
            }
            catch (Exception)
            {
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = null,
                };
            }
        }

        public async Task<APIResponse<TResponse>> PatchAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _options);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync(url, content);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                };

            // Response failed
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_options);

                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = error
                };
            }
            
            try
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = result,
                    ErrorResponse = null,
                };
            }
            catch (Exception)
            {
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = null,
                };
            }
        }

        public async Task<APIResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _options);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url, content);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                };

            // Response failed
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_options);

                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = error
                };
            }
            
            try
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = result,
                    ErrorResponse = null,
                };
            }
            catch (Exception)
            {
                return new APIResponse<TResponse>
                {
                    Status = response.StatusCode,
                    Content = default,
                    ErrorResponse = null,
                };
            }
        }


        public async Task<HttpStatusCode> DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);

            return response.StatusCode;
        }

    }
}
