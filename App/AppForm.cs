using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class AppForm : Form
    {

        private IServiceProvider _serviceProvider;
        private INavigationService _navigationService;
        private AccountManager _accountManager;

        // Controls
        private Panel _panelContainer;

        public AppForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            InitializeControls();

            _serviceProvider = serviceProvider;
            _accountManager = serviceProvider.GetRequiredService<AccountManager>();
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
