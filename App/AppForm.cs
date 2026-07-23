using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public partial class AppForm : Form
    {
        private INavigationService _navigationService;

        // Controls
        private Panel _panelContainer;

        public AppForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Form settings
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 600);

            InitializeControls();

            _navigationService = serviceProvider.GetRequiredService<INavigationService>();

            _navigationService.Initialize(_panelContainer);

            _navigationService.ShowView<LoginView>();
        }

        private void InitializeControls()
        {
            _panelContainer = new Panel
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_panelContainer);
        }
    }
}