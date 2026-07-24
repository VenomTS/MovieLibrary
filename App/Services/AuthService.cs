using App.Services.Interfaces;
using DTO.Users;
using System.Net;
using App.Account;
using App.APIResponses;

namespace App.Services
{
    public class AuthService(IHttpService httpService, AccountManager accountManager) : IAuthService
    {
        public async Task<APIResponse<LoginResponse>> LoginAsync(string mail, string password)
        {
            var response = await httpService.PostAsync<LoginRequest, LoginResponse>("auth/login", new LoginRequest
            {
                Email = mail,
                Password = password
            });
            
            Console.WriteLine(response.Content!.AccessToken);

            if (response.Status == HttpStatusCode.OK)
                await accountManager.Initialize(response.Content!.AccessToken);
            
            return response;
        }

        public async Task<APIResponse<EmptyResponse>> RegisterAsync(string mail, string password)
        {
            var response = await httpService.PostAsync<RegisterRequest, EmptyResponse>("auth/register",
                new RegisterRequest
                {
                    Email = mail,
                    Password = password
                });

            return response;
        }
    }
}
