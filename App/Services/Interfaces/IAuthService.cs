namespace App.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> LoginAsync(string mail, string password);
        public Task<bool> RegisterAsync(string mail, string password);
    }
}
