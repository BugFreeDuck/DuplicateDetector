using System.Configuration;
using System.Data;
using System.Security.Authentication.ExtendedProtection;
using System.Windows;
using DuplicateDetector.WinApp.DuplicateDetection;
using Microsoft.Extensions.DependencyInjection;

namespace DuplicateDetector.WinApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.ShowDialog();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<DuplicateDetectionService>();
        services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
