using App.Services.Interfaces;
using System.CodeDom;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DTO;

namespace App.Services
{
    public class HttpService : IHttpService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
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

        public async Task<(HttpStatusCode, TResponse?)> GetAsync<TResponse>(string url)
        {
            var response = await _client.GetAsync(url);

            var isSuccessful = await IsSuccessfulAsync(response);

            if (!isSuccessful)
                return (response.StatusCode, default);

            var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
            return (response.StatusCode, result);
        }

        public async Task<(HttpStatusCode, TResponse?)> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _client.PostAsJsonAsync(url, data, _options);

            var isSuccessful = await IsSuccessfulAsync(response);

            if (!isSuccessful || typeof(TResponse) == typeof(EmptyResponse))
                return (response.StatusCode, (TResponse)(object)new EmptyResponse
                {
                    ResponseText = response.Content.ReadAsStringAsync().Result
                });

            var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);

            return (response.StatusCode, result);
        }
        
        public async Task<(HttpStatusCode, TResponse?)> PatchAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _options);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync(url, content);

            var isSuccessful = await IsSuccessfulAsync(response);
            if (!isSuccessful)
                return (response.StatusCode, default(TResponse));

            var result = await response.Content.ReadFromJsonAsync<TResponse>(_options);
            return (response.StatusCode, result);
        }


        public async Task<HttpStatusCode> DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);

            return response.StatusCode;
        }

        private static async Task<bool> IsSuccessfulAsync(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode;
        }

    }
}
