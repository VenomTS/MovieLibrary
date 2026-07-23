using App.APIResponses;
using DTO.Users;

namespace App.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<APIResponse<LoginResponse>> LoginAsync(string mail, string password);
        public Task<APIResponse<EmptyResponse>> RegisterAsync(string mail, string password);
    }
}
