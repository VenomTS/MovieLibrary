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
            
            // ApplicationConfiguration.Initialize();
            Application.Run(ServiceProvider.GetRequiredService<MainPage>());
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddTransient<MainPage>();
            });
        }
    }
}