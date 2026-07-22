using App.Services.Interfaces;
using DTO.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace App.Services
{
    public class AuthService(IHttpService httpService, AccountManager accountManager) : IAuthService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            (HttpStatusCode statusCode, LoginResponse? content) = await httpService.PostAsync<LoginRequest, LoginResponse>("auth/login", new LoginRequest
            {
                Username = username,
                Password = password
            });

            if (statusCode == HttpStatusCode.OK)
                await accountManager.Initialize(content!.JsonWebToken);


            return statusCode == HttpStatusCode.OK;
        }
    }
}
