using System.Net;
using System.Security.Cryptography.Pkcs;
using App.APIResponses;

namespace App.Services.Interfaces
{
    public interface IHttpService
    {
        public void SetJwt(string jwt);
        public Task<APIResponse<TResponse>> GetAsync<TResponse>(string url);
        public Task<APIResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data);
        public Task<APIResponse<TResponse>> PatchAsync<TRequest, TResponse>(string url, TRequest data);
        public Task<APIResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data);
        public Task<HttpStatusCode> DeleteAsync(string url);
    }
}
