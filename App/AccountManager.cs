using App.Services.Interfaces;
using DTO.Rentals;
using DTO.Users;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace App
{
    public class AccountManager(IHttpService httpService)
    {
        public User? User { get; set; } = null;

        public async Task Initialize(string jwt)
        {
            httpService.SetJwt(jwt);
            var (statusCode, user) = await httpService.GetAsync<GetMeResponse>("auth/me");

            if (statusCode != HttpStatusCode.OK)
                throw new Exception("Account is not authorized");

            SetupUser(user);
        }

        public bool IsLoggedIn() => User != null;

        private void SetupUser(GetMeResponse user)
        {
            User = new User
            {
                Id = user.Id,
                Role = user.Role
            };
        }

    }
}
