using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

namespace OFSApp;

static class Program
{
    
    private static IServiceProvider _serviceProvider;
    
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        var host = CreateHostBuilder().Build();
        _serviceProvider = host.Services;
        
        var appForm = _serviceProvider.GetRequiredService<FiscalSettings>();
        Application.Run(appForm);
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            services.AddTransient<FiscalSettings>();

            services.AddSingleton<OFSService>();
        });
    }
}