using System.Net;

namespace App.Services.Interfaces
{
    public interface IHttpService
    {
        public void SetJwt(string jwt);
        public Task<(HttpStatusCode, TResponse?)> GetAsync<TResponse>(string url);
        public Task<(HttpStatusCode, TResponse?)> PostAsync<TRequest, TResponse>(string url, TRequest data);
        public Task<HttpStatusCode> DeleteAsync(string url);
        public Task<(HttpStatusCode, TResponse?)> PatchAsync<TRequest, TResponse>(string url, TRequest data);
    }
}
