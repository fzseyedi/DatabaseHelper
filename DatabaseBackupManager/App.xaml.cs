using System.Windows;
using DatabaseBackupManager.Services;
using DatabaseBackupManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;

using Application = System.Windows.Application;

namespace DatabaseBackupManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the current application instance.
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public IServiceProvider Services => _serviceProvider!;

    /// <summary>
    /// Configures the dependency injection container.
    /// </summary>
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<ISqlServerService, SqlServerService>();

        // Register ViewModels
        services.AddSingleton<MainViewModel>();

        // Register Views
        services.AddTransient<MainWindow>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Application startup event handler.
    /// </summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _serviceProvider = ConfigureServices();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Application exit event handler.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

