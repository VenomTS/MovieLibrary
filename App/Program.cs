using App.Account;
using App.Dialogs;
using App.Services;
using App.Services.Interfaces;
using App.UserControls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            var appForm = ServiceProvider.GetRequiredService<AppForm>();
            Application.Run(appForm);
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                // Services
                services.AddTransient<IHttpService, HttpService>();
                services.AddTransient<IAuthService, AuthService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // 
                services.AddSingleton<AccountManager>();

                // Forms
                services.AddSingleton<AppForm>();
                
                // Dialogs
                services.AddTransient<AddMovieDialog>();
                services.AddTransient<InventoryDialog>();

                // UserControls
                services.AddTransient<LoginView>();
                services.AddTransient<RegisterView>();
                services.AddTransient<RentMoviesView>();
                services.AddTransient<InventoryManagementView>();
                services.AddTransient<MyRentalsView>();
                services.AddTransient<AccountManagementView>();
            });
        }
    }
}