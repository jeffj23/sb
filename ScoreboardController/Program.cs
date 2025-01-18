using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Scoreboard.Data;
using ScoreboardController.Data;
using ScoreboardController.Helpers;
using ScoreboardController.Views;
using ScoreboardController.Services;
using ScoreboardController.ViewModels;
using ScoreDbContext = ScoreboardController.Data.ScoreDbContext;

namespace ScoreboardController
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 1) Register your DbContext (replace with your connection string)
                    services.AddDbContext<ScoreDbContext>(options =>
                    {
                        options.UseSqlServer("Server=localhost;Database=Score;Trusted_Connection=True;");
                    });

                    // 2) Register services that load softkeys, handle JSON sending, etc.
                    services.AddSingleton<ISoftKeyService, SoftKeyService>();
                    services.AddSingleton<IJsonMessenger, JsonMessenger>();
                    services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<TupleConverter>();
                    // 3) Register MainWindow
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<ICommandMappingService, MockCommandMappingService>();
                    services.AddSingleton<ISoftKeyRepository, MockSoftKeyRepository>();
                    services.AddSingleton<ITimerService, PrecisionTimerService>();


                })
                .Build();

            // Create a standard WPF Application object
            var app = new Application();

            // Merge the dictionary from Resource (now a ResourceDictionary)
            var resDict = new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/ScoreboardController;component/Resource.xaml",
                    UriKind.Absolute)
            };
            app.Resources.MergedDictionaries.Add(resDict);

            // Resolve MainWindow from DI
            var mainWindow = host.Services.GetRequiredService<MainWindow>();

            // Run the WPF app
            app.Run(mainWindow);
        }
    }
}