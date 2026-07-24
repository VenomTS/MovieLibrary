using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services
{
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        private Panel _panel;

        public event EventHandler<OnNavigatedToArgs>? OnNavigatedTo;

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
            
            OnNavigatedTo?.Invoke(this, new OnNavigatedToArgs
            {
                NavigatedTo = view
            });
        }
    }
}
