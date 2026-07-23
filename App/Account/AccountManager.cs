using System.Net;
using App.Services.Interfaces;
using DTO.Users;

namespace App.Account
{
    public class AccountManager(IHttpService httpService)
    {
        public AppAccount? User { get; set; }

        public async Task Initialize(string jwt)
        {
            httpService.SetJwt(jwt);
            var response = await httpService.GetAsync<GetMeResponse>("auth/me");
            
            if(response.Status != HttpStatusCode.OK)
                throw new Exception("Account is not authorized");

            if (response.Content == null)
                return;

            SetupUser(response.Content);
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
