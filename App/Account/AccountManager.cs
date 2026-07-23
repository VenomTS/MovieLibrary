using System.Net;
using App.Services.Interfaces;
using DTO.Users;

namespace App.Account
{
    public class AccountManager(IHttpService httpService)
    {
        public AppAccount? User { get; set; } = null;

        public async Task Initialize(string jwt)
        {
            httpService.SetJwt(jwt);
            var (statusCode, user) = await httpService.GetAsync<GetMeResponse>("auth/me");

            if (statusCode != HttpStatusCode.OK)
                throw new Exception("Account is not authorized");

            if (user == null)
                return;

            SetupUser(user);
        }

        public bool IsLoggedIn() => User != null;

        private void SetupUser(GetMeResponse user)
        {
            User = new AppAccount
            {
                Id = user.Id,
                Roles = user.Roles
            };
        }

    }
}
