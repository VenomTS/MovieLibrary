using DTO.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Services
{
    public class AuthService(HttpClientService httpService)
    {
        public async Task<bool> Login(string username, string password)
        {
            var token = httpService.PostAsync<LoginRequest, string>("", new LoginRequest
            {
                Username = username,
                Password = password
            });
            return true;
        }
    }
}
