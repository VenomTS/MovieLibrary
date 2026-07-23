using App.Services.Interfaces;
using DTO.Users;
using System.Net;
using App.Account;
using DTO;

namespace App.Services
{
    public class AuthService(IHttpService httpService, AccountManager accountManager) : IAuthService
    {
        public async Task<bool> LoginAsync(string mail, string password)
        {
            var (statusCode, content) = await httpService.PostAsync<LoginRequest, LoginResponse>("auth/login", new LoginRequest
            {
                Email = mail,
                Password = password
            });

            if (statusCode == HttpStatusCode.OK)
                await accountManager.Initialize(content!.AccessToken);


            return statusCode == HttpStatusCode.OK;
        }

        public async Task<bool> RegisterAsync(string mail, string password)
        {
            var (statusCode, content) = await httpService.PostAsync<RegisterRequest, EmptyResponse>("auth/register",
                new RegisterRequest
                {
                    Email = mail,
                    Password = password
                });
            
            return statusCode == HttpStatusCode.OK;
        }
    }
}
