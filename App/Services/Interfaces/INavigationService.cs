using System;
using System.Collections.Generic;
using System.Text;

namespace App.Services.Interfaces
{
    public interface INavigationService
    {
        public void Initialize(Panel panel);
        public void ShowView<T>() where T : UserControl;
    }
}
