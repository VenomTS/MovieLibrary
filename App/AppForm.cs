using App.Account;
using App.Services;
using App.Services.Interfaces;
using App.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public partial class AppForm : Form
{
    private readonly INavigationService _navigationService;
    private readonly AccountManager _accountManager;

    private Button rentsButton;
    private Button movieManagementButton;
    private Button myRentalsButton;
    private Button logoutButton;
    private Button accountManagementButton;

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
        _accountManager = serviceProvider.GetRequiredService<AccountManager>();

        _navigationService.Initialize(_panelContainer);
        _navigationService.OnNavigatedTo += OnNavigatedTo;
        
        SetupButtonPermissions();

        _navigationService.ShowView<LoginView>();
    }

    private void OnNavigatedTo(object? sender, OnNavigatedToArgs e)
    {
        var view = e.NavigatedTo;
        
        _topPanel.Visible = view is not (LoginView or RegisterView);
        
        if(!_accountManager.IsLoggedIn() && view is not (LoginView or RegisterView))
            _navigationService.ShowView<LoginView>();
        
        if(view is not (LoginView or RegisterView))
            SetupButtonPermissions();
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
        rentsButton = CreateNavButton("Rent");
        myRentalsButton = CreateNavButton("My Rentals");
        movieManagementButton = CreateNavButton("Movie Management");
        accountManagementButton = CreateNavButton("Account Management");
        logoutButton = CreateNavButton("Logout");

        rentsButton.Click += (_, _) => _navigationService.ShowView<RentMoviesView>();
        myRentalsButton.Click += (_, _) => _navigationService.ShowView<MyRentalsView>();
        movieManagementButton.Click += (_, _) => _navigationService.ShowView<InventoryManagementView>();
        accountManagementButton.Click += (_, _) => _navigationService.ShowView<AccountManagementView>();
        
        logoutButton.Click += LogoutButtonOnClick;

        // Add buttons left-to-right
        _topPanel.Controls.Add(logoutButton);
        _topPanel.Controls.Add(accountManagementButton);
        _topPanel.Controls.Add(movieManagementButton);
        _topPanel.Controls.Add(myRentalsButton);
        _topPanel.Controls.Add(rentsButton);
        

        Controls.Add(_panelContainer);
        Controls.Add(_topPanel);
    }

    private void SetupButtonPermissions()
    {
        // Logout, Renting and MyRentals moze svako
        // Movie Management moze Librarian
        
        movieManagementButton.Visible = _accountManager.HasRole("Librarian");
        accountManagementButton.Visible = _accountManager.HasRole("Admin");
    }

    private void LogoutButtonOnClick(object? sender, EventArgs e)
    {
        _accountManager.LogOut();
        _navigationService.ShowView<LoginView>();
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