using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace App.Services
{
    public class HttpClientService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpClient _client;

        public HttpClientService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _client = new HttpClient(handler);
        }

        public async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            var response = await _client.GetAsync(url);

            await EnsureSuccess(response);

            return await response.Content
                .ReadFromJsonAsync<TResponse>(_options);
        }


        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string url,
            TRequest data)
        {
            var response = await _client.PostAsJsonAsync(
                url,
                data,
                _options);

            await EnsureSuccess(response);

            return await response.Content
                .ReadFromJsonAsync<TResponse>(_options);
        }


        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(
            string url,
            TRequest data)
        {
            var json = JsonSerializer.Serialize(data, _options);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _client.PatchAsync(url, content);

            await EnsureSuccess(response);

            return await response.Content
                .ReadFromJsonAsync<TResponse>(_options);
        }


        public async Task DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);

            await EnsureSuccess(response);
        }


        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"HTTP request failed: {(int)response.StatusCode} - {error}");
            }
        }

    }
}
