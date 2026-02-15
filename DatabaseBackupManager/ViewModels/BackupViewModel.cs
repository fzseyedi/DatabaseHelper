using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for database backup operations.
/// </summary>
public class BackupViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionViewModel _connectionViewModel;
    private readonly BackupSettingsManager _settingsManager;
    private DatabaseInfo? _selectedDatabase;
    private BackupType _selectedBackupType = BackupType.Full;
    private string _backupPath = string.Empty;
    private bool _compressBackup = true;
    private bool _rememberSettings = true;
    private int _progress;
    private bool _isBackupInProgress;
    private CancellationTokenSource? _cancellationTokenSource;

    public BackupViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
    {
        _sqlServerService = sqlServerService;
        _connectionViewModel = connectionViewModel;
        _settingsManager = new BackupSettingsManager();

        // Initialize commands
        BackupCommand = new RelayCommand(async () => await BackupAsync(), CanBackup);
        CancelBackupCommand = new RelayCommand(CancelBackup, () => IsBackupInProgress);
        BrowseBackupPathCommand = new RelayCommand(BrowseBackupPath);
        RefreshDatabasesCommand = new RelayCommand(async () => await _connectionViewModel.LoadDatabasesAsync(), 
            () => _connectionViewModel.IsConnected && !IsBusy);
        ClearSettingsCommand = new RelayCommand(ClearSavedSettings);

        // Set default backup path
        BackupPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Load saved backup settings
        _ = LoadSavedSettingsAsync();
    }

    /// <summary>
    /// Gets or sets whether to remember backup settings.
    /// </summary>
    public bool RememberSettings
    {
        get => _rememberSettings;
        set => SetProperty(ref _rememberSettings, value);
    }


    /// <summary>
    /// Gets the list of databases from the connection view model.
    /// </summary>
    public ObservableCollection<DatabaseInfo> Databases => _connectionViewModel.Databases;

    /// <summary>
    /// Gets or sets the selected database for backup.
    /// </summary>
    public DatabaseInfo? SelectedDatabase
    {
        get => _selectedDatabase;
        set => SetProperty(ref _selectedDatabase, value);
    }

    /// <summary>
    /// Gets the available backup types.
    /// </summary>
    public IEnumerable<BackupType> BackupTypes => Enum.GetValues<BackupType>();

    /// <summary>
    /// Gets or sets the selected backup type.
    /// </summary>
    public BackupType SelectedBackupType
    {
        get => _selectedBackupType;
        set => SetProperty(ref _selectedBackupType, value);
    }

    /// <summary>
    /// Gets or sets the backup destination path.
    /// </summary>
    public string BackupPath
    {
        get => _backupPath;
        set => SetProperty(ref _backupPath, value);
    }

    /// <summary>
    /// Gets or sets whether to compress the backup.
    /// </summary>
    public bool CompressBackup
    {
        get => _compressBackup;
        set => SetProperty(ref _compressBackup, value);
    }

    /// <summary>
    /// Gets or sets the backup progress (0-100).
    /// </summary>
    public int Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    /// <summary>
    /// Gets or sets whether a backup is currently in progress.
    /// </summary>
    public bool IsBackupInProgress
    {
        get => _isBackupInProgress;
        private set => SetProperty(ref _isBackupInProgress, value);
    }

    // Commands
    public ICommand BackupCommand { get; }
    public ICommand CancelBackupCommand { get; }
    public ICommand BrowseBackupPathCommand { get; }
    public ICommand RefreshDatabasesCommand { get; }
    public ICommand ClearSettingsCommand { get; }

    private bool CanBackup() => 
        SelectedDatabase != null && 
        !string.IsNullOrWhiteSpace(BackupPath) && 
        !IsBackupInProgress &&
        _connectionViewModel.IsConnected;

    /// <summary>
    /// Performs the database backup.
    /// </summary>
    private async Task BackupAsync()
    {
        if (SelectedDatabase == null) return;

        IsBackupInProgress = true;
        IsBusy = true;
        Progress = 0;
        ClearError();

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var extension = SelectedBackupType == BackupType.TransactionLog ? ".trn" : ".bak";
            var fileName = $"{SelectedDatabase.Name}_{SelectedBackupType}_{timestamp}{extension}";
            var fullPath = Path.Combine(BackupPath, fileName);

            StatusMessage = $"در حال پشتیبان‌گیری از {SelectedDatabase.Name}...";

            var progress = new Progress<int>(p =>
            {
                Progress = p;
                StatusMessage = $"در حال پشتیبان‌گیری از {SelectedDatabase.Name}... {p}%";
            });

            await _sqlServerService.BackupDatabaseAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedDatabase.Name,
                fullPath,
                SelectedBackupType,
                CompressBackup,
                progress,
                _cancellationTokenSource.Token);

            Progress = 100;
            StatusMessage = $"پشتیبان‌گیری با موفقیت انجام شد: {fileName}";

            // Save backup settings if remember is enabled
            await SaveCurrentSettingsAsync();

            // Refresh database list to update last backup date
            await _connectionViewModel.LoadDatabasesAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "پشتیبان‌گیری لغو شد.";
            Progress = 0;
        }
        catch (Exception ex)
        {
            SetError($"پشتیبان‌گیری ناموفق بود: {ex.Message}");
            Progress = 0;
        }
        finally
        {
            IsBackupInProgress = false;
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Cancels the current backup operation.
    /// </summary>
    private void CancelBackup()
    {
        _cancellationTokenSource?.Cancel();
        StatusMessage = "در حال لغو پشتیبان‌گیری...";
    }

    /// <summary>
    /// Opens a folder browser dialog to select the backup path.
    /// </summary>
    private void BrowseBackupPath()
    {
        // استفاده از Windows Forms FolderBrowserDialog
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "انتخاب پوشه مقصد پشتیبان‌گیری",
            SelectedPath = BackupPath,
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            BackupPath = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// Loads previously saved backup settings.
    /// </summary>
    private async Task LoadSavedSettingsAsync()
    {
        try
        {
            var savedSettings = await _settingsManager.LoadBackupSettingsAsync();
            if (savedSettings != null)
            {
                BackupPath = savedSettings.BackupPath;
                CompressBackup = savedSettings.CompressBackup;
                if (Enum.TryParse<BackupType>(savedSettings.SelectedBackupType, out var backupType))
                {
                    SelectedBackupType = backupType;
                }
                StatusMessage = "تنظیمات پشتیبان‌گیری ذخیره شده بارگذاری شد.";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در بارگذاری تنظیمات پشتیبان‌گیری: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the current backup settings.
    /// </summary>
    private async Task SaveCurrentSettingsAsync()
    {
        if (RememberSettings)
        {
            await _settingsManager.SaveBackupSettingsAsync(BackupPath, CompressBackup, SelectedBackupType.ToString());
        }
    }

    /// <summary>
    /// Clears saved backup settings.
    /// </summary>
    private void ClearSavedSettings()
    {
        _settingsManager.DeleteBackupSettings();
        StatusMessage = "تنظیمات پشتیبان‌گیری ذخیره شده پاک شد.";
    }
}
