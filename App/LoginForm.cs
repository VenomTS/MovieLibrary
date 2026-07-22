using App.Services;
using App.Services.Interfaces;

namespace App
{
    public partial class LoginForm : Form
    {
        

        private readonly IAuthService _authService;

        public LoginForm(IAuthService authService)
        {
            _authService = authService;
            
        }
    }
}
