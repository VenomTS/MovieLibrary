using DTO.OFS.ResponseObject;
using Services.ResponseObject;

namespace Services;

public interface IHttpService
{
    public Task<HttpResponseObject<TResponse>> PostJsonAsync<TRequest, TResponse>(string url, TRequest request, IDictionary<string, string>? headers = null);
    public Task<HttpResponse> PostJsonAsync<TRequest>(string url, TRequest request);

    // public Task<HttpResponseObject<string>> PostTextAsync(string url, string request);

    public Task<HttpResponseObject<TResponse>> GetAsync<TResponse>(string url);
    public Task<HttpResponse> GetAsync(string url);
}