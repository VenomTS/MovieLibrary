using System;
using System.Collections.Generic;
using System.Text;

namespace App.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> LoginAsync(string username, string password);
    }
}
