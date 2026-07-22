using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Services
{
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        private Panel _panel;
        public void Initialize(Panel panel)
        {
            _panel = panel;
        }

        public void ShowView<T>() where T : UserControl
        {
            var view = serviceProvider.GetRequiredService<T>();
            _panel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _panel.Controls.Add(view);
        }
    }
}
