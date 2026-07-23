using App.Services;
using App.Services.Interfaces;
using App.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public partial class AppForm : Form
{
    private readonly INavigationService _navigationService;

    // Controls
    private Panel _topPanel;
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
        _navigationService.OnNavigatedTo += OnNavigatedTo;

        _navigationService.ShowView<LoginView>();
    }

    private void OnNavigatedTo(object? sender, OnNavigatedToArgs e)
    {
        var view = e.NavigatedTo;
        
        _topPanel.Visible = view is not LoginView or RegisterView;
    }

    private void InitializeControls()
    {
        // Top navigation bar
        _topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(45, 45, 48)
        };

        // Main content area
        _panelContainer = new Panel
        {
            Dock = DockStyle.Fill
        };

        // Navigation buttons
        var btnRent = CreateNavButton("Rent");
        btnRent.Click += (_, _) => _navigationService.ShowView<RentMoviesView>();

        var btnInventory = CreateNavButton("Inventory");
        btnInventory.Click += (_, _) => _navigationService.ShowView<MovieInventoryView>();

        /*
        var btnCustomers = CreateNavButton("Customers");
        btnCustomers.Click += (_, _) => _navigationService.ShowView<CustomersView>();

        var btnReports = CreateNavButton("Reports");
        btnReports.Click += (_, _) => _navigationService.ShowView<ReportsView>();
        */

        var btnSettings = CreateNavButton("Rentals");
        btnSettings.Click += (_, _) => _navigationService.ShowView<RentalsView>();

        // Add buttons left-to-right
        _topPanel.Controls.Add(btnSettings);
        _topPanel.Controls.Add(btnInventory);
        _topPanel.Controls.Add(btnRent);

        Controls.Add(_panelContainer);
        Controls.Add(_topPanel);
    }

    private Button CreateNavButton(string text)
    {
        return new Button
        {
            Text = text,
            Dock = DockStyle.Left,
            Width = 120,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            ForeColor = Color.White,
            BackColor = Color.FromArgb(45, 45, 48),
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };
    }
}
}