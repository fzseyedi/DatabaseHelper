using System.Collections.ObjectModel;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for managing SQL Server connection.
/// </summary>
public class ConnectionViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionSettingsManager _settingsManager;
    private bool _isConnected;
    private string _connectionStatus = "قطع ارتباط";
    private bool _rememberSettings = true;

    public ConnectionViewModel(ISqlServerService sqlServerService)
    {
        _sqlServerService = sqlServerService;
        _settingsManager = new ConnectionSettingsManager();
        ConnectionSettings = new ConnectionSettings();
        Databases = new ObservableCollection<DatabaseInfo>();

        // Initialize commands
        TestConnectionCommand = new RelayCommand(async () => await TestConnectionAsync(), CanTestConnection);
        ConnectCommand = new RelayCommand(async () => await ConnectAsync(), CanConnect);
        DisconnectCommand = new RelayCommand(Disconnect, () => IsConnected);
        RefreshDatabasesCommand = new RelayCommand(async () => await LoadDatabasesAsync(), () => IsConnected);
        ClearSettingsCommand = new RelayCommand(ClearSavedSettings);

        // Load saved settings on initialization
        _ = LoadSavedSettingsAsync();
    }

    /// <summary>
    /// Gets or sets whether to remember connection settings.
    /// </summary>
    public bool RememberSettings
    {
        get => _rememberSettings;
        set => SetProperty(ref _rememberSettings, value);
    }


    /// <summary>
    /// Gets the connection settings.
    /// </summary>
    public ConnectionSettings ConnectionSettings { get; }

    /// <summary>
    /// Gets the list of databases on the server.
    /// </summary>
    public ObservableCollection<DatabaseInfo> Databases { get; }

    /// <summary>
    /// Gets or sets whether the connection is established.
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (SetProperty(ref _isConnected, value))
            {
                ConnectionStatus = value ? $"متصل شد به {ConnectionSettings.ServerName}" : "قطع ارتباط";
            }
        }
    }

    /// <summary>
    /// Gets or sets the connection status message.
    /// </summary>
    public string ConnectionStatus
    {
        get => _connectionStatus;
        private set => SetProperty(ref _connectionStatus, value);
    }

    // Commands
    public ICommand TestConnectionCommand { get; }
    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand RefreshDatabasesCommand { get; }
    public ICommand ClearSettingsCommand { get; }

    private bool CanTestConnection() => !string.IsNullOrWhiteSpace(ConnectionSettings.ServerName) && !IsBusy;
    private bool CanConnect() => !string.IsNullOrWhiteSpace(ConnectionSettings.ServerName) && !IsBusy && !IsConnected;

    /// <summary>
    /// Gets the current password from connection settings.
    /// Used by the view to populate the PasswordBox after async loading.
    /// </summary>
    public string GetCurrentPassword() => ConnectionSettings.Password;

    /// <summary>
    /// Tests the connection to the SQL Server.
    /// </summary>
    private async Task TestConnectionAsync()
    {
        IsBusy = true;
        ClearError();
        StatusMessage = "در حال تست اتصال...";

        try
        {
            var success = await _sqlServerService.TestConnectionAsync(ConnectionSettings);
            
            if (success)
            {
                StatusMessage = "تست اتصال موفق بود!";
            }
            else
            {
                SetError("تست اتصال ناموفق بود. لطفا تنظیمات خود را بررسی کنید.");
            }
        }
        catch (Exception ex)
        {
            SetError($"تست اتصال ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Connects to the SQL Server and loads databases.
    /// </summary>
    private async Task ConnectAsync()
    {
        IsBusy = true;
        ClearError();
        StatusMessage = "در حال اتصال...";

        try
        {
            var success = await _sqlServerService.TestConnectionAsync(ConnectionSettings);
            
            if (success)
            {
                IsConnected = true;
                await LoadDatabasesAsync();
                await SaveCurrentSettingsAsync();
                StatusMessage = $"متصل شد. {Databases.Count} پایگاه داده پیدا شد.";
            }
            else
            {
                SetError("اتصال ناموفق بود. لطفا تنظیمات خود را بررسی کنید.");
            }
        }
        catch (Exception ex)
        {
            SetError($"اتصال ناموفق شد: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Disconnects from the SQL Server.
    /// </summary>
    private void Disconnect()
    {
        IsConnected = false;
        Databases.Clear();
        StatusMessage = "قطع ارتباط شد.";
        ClearError();
    }

    /// <summary>
    /// Loads previously saved connection settings.
    /// </summary>
    private async Task LoadSavedSettingsAsync()
    {
        try
        {
            var savedSettings = await _settingsManager.LoadConnectionSettingsAsync();
            if (savedSettings != null)
            {
                ConnectionSettings.ServerName = savedSettings.ServerName;
                ConnectionSettings.AuthenticationType = savedSettings.AuthenticationType;
                ConnectionSettings.Username = savedSettings.Username;
                ConnectionSettings.Password = savedSettings.Password;
                ConnectionSettings.ConnectionTimeout = savedSettings.ConnectionTimeout;
                ConnectionSettings.TrustServerCertificate = savedSettings.TrustServerCertificate;
                StatusMessage = "تنظیمات اتصال ذخیره شده قبلی بارگذاری شد.";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در بارگذاری تنظیمات ذخیره شده: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the current connection settings.
    /// </summary>
    private async Task SaveCurrentSettingsAsync()
    {
        if (RememberSettings)
        {
            await _settingsManager.SaveConnectionSettingsAsync(ConnectionSettings);
        }
    }

    /// <summary>
    /// Clears saved connection settings.
    /// </summary>
    private void ClearSavedSettings()
    {
        _settingsManager.DeleteConnectionSettings();
        StatusMessage = "تنظیمات اتصال ذخیره شده پاک شد.";
    }

    /// <summary>
    /// Loads the list of databases from the server.
    /// </summary>
    public async Task LoadDatabasesAsync()
    {
        if (!IsConnected) return;

        IsBusy = true;
        StatusMessage = "در حال بارگذاری پایگاه‌های داده...";

        try
        {
            var databases = await _sqlServerService.GetDatabasesAsync(ConnectionSettings);
            
            Databases.Clear();
            foreach (var db in databases)
            {
                Databases.Add(db);
            }

            StatusMessage = $"{Databases.Count} پایگاه داده بارگذاری شد.";
        }
        catch (Exception ex)
        {
            SetError($"بارگذاری پایگاه‌های داده ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
