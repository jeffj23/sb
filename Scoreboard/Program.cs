using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scoreboard.Data;

namespace Scoreboard;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContext<ScoreDbContext>(options =>
                    SqlServerDbContextOptionsExtensions.UseSqlServer(options, "Server=DESKTOP-CN7A5TA;Database=Score;Trusted_Connection=True;TrustServerCertificate=True;"));

                services.AddSingleton<MainWindow>();
            })
            .Build();

        var app = new Application();
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        app.Run(mainWindow);
    }
}