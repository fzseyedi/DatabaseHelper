using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for database restore operations.
/// </summary>
public class RestoreViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionViewModel _connectionViewModel;
    private string _backupFilePath = string.Empty;
    private string _targetDatabaseName = string.Empty;
    private string? _dataFilePath;
    private string? _logFilePath;
    private bool _restoreAsNewDatabase;
    private int _progress;
    private bool _isRestoreInProgress;
    private CancellationTokenSource? _cancellationTokenSource;
    private BackupFileInfo? _selectedBackupFile;

    public RestoreViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
    {
        _sqlServerService = sqlServerService;
        _connectionViewModel = connectionViewModel;

        BackupFileList = [];

        // Initialize commands
        RestoreCommand = new RelayCommand(async () => await RestoreAsync(), CanRestore);
        CancelRestoreCommand = new RelayCommand(CancelRestore, () => IsRestoreInProgress);
        BrowseBackupFileCommand = new RelayCommand(BrowseBackupFile);
        BrowseDataFilePathCommand = new RelayCommand(BrowseDataFilePath);
        BrowseLogFilePathCommand = new RelayCommand(BrowseLogFilePath);
        LoadBackupInfoCommand = new RelayCommand(async () => await LoadBackupInfoAsync(), CanLoadBackupInfo);
    }

    /// <summary>
    /// Gets the list of databases from the connection view model.
    /// </summary>
    public ObservableCollection<DatabaseInfo> Databases => _connectionViewModel.Databases;

    /// <summary>
    /// Gets the list of files contained in the backup.
    /// </summary>
    public ObservableCollection<BackupFileInfo> BackupFileList { get; }

    /// <summary>
    /// Gets or sets the selected backup file info.
    /// </summary>
    public BackupFileInfo? SelectedBackupFile
    {
        get => _selectedBackupFile;
        set => SetProperty(ref _selectedBackupFile, value);
    }

    /// <summary>
    /// Gets or sets the path to the backup file to restore.
    /// </summary>
    public string BackupFilePath
    {
        get => _backupFilePath;
        set
        {
            if (SetProperty(ref _backupFilePath, value))
            {
                // Auto-extract database name from backup file name
                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    var fileName = Path.GetFileNameWithoutExtension(value);
                    var parts = fileName.Split('_');
                    if (parts.Length > 0 && string.IsNullOrEmpty(TargetDatabaseName))
                    {
                        TargetDatabaseName = parts[0];
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the target database name for the restore.
    /// </summary>
    public string TargetDatabaseName
    {
        get => _targetDatabaseName;
        set => SetProperty(ref _targetDatabaseName, value);
    }

    /// <summary>
    /// Gets or sets the optional data file path for the restore.
    /// </summary>
    public string? DataFilePath
    {
        get => _dataFilePath;
        set => SetProperty(ref _dataFilePath, value);
    }

    /// <summary>
    /// Gets or sets the optional log file path for the restore.
    /// </summary>
    public string? LogFilePath
    {
        get => _logFilePath;
        set => SetProperty(ref _logFilePath, value);
    }

    /// <summary>
    /// Gets or sets whether to restore as a new database.
    /// </summary>
    public bool RestoreAsNewDatabase
    {
        get => _restoreAsNewDatabase;
        set => SetProperty(ref _restoreAsNewDatabase, value);
    }

    /// <summary>
    /// Gets or sets the restore progress (0-100).
    /// </summary>
    public int Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    /// <summary>
    /// Gets or sets whether a restore is currently in progress.
    /// </summary>
    public bool IsRestoreInProgress
    {
        get => _isRestoreInProgress;
        private set => SetProperty(ref _isRestoreInProgress, value);
    }

    // Commands
    public ICommand RestoreCommand { get; }
    public ICommand CancelRestoreCommand { get; }
    public ICommand BrowseBackupFileCommand { get; }
    public ICommand BrowseDataFilePathCommand { get; }
    public ICommand BrowseLogFilePathCommand { get; }
    public ICommand LoadBackupInfoCommand { get; }

    private bool CanRestore() =>
        !string.IsNullOrWhiteSpace(BackupFilePath) &&
        File.Exists(BackupFilePath) &&
        !string.IsNullOrWhiteSpace(TargetDatabaseName) &&
        !IsRestoreInProgress &&
        _connectionViewModel.IsConnected;

    private bool CanLoadBackupInfo() =>
        !string.IsNullOrWhiteSpace(BackupFilePath) &&
        File.Exists(BackupFilePath) &&
        !IsBusy &&
        _connectionViewModel.IsConnected;

    /// <summary>
    /// Loads backup file information.
    /// </summary>
    private async Task LoadBackupInfoAsync()
    {
        if (string.IsNullOrWhiteSpace(BackupFilePath)) return;

        IsBusy = true;
        ClearError();
        BackupFileList.Clear();

        try
        {
            StatusMessage = "Loading backup file information...";

            var fileList = await _sqlServerService.GetBackupFileListAsync(
                _connectionViewModel.ConnectionSettings,
                BackupFilePath);

            foreach (var file in fileList)
            {
                BackupFileList.Add(file);
            }

            StatusMessage = $"Found {fileList.Count} file(s) in backup.";
        }
        catch (Exception ex)
        {
            SetError($"Failed to load backup info: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Performs the database restore.
    /// </summary>
    private async Task RestoreAsync()
    {
        if (string.IsNullOrWhiteSpace(BackupFilePath) || string.IsNullOrWhiteSpace(TargetDatabaseName)) return;

        IsRestoreInProgress = true;
        IsBusy = true;
        Progress = 0;
        ClearError();

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            StatusMessage = $"Restoring database {TargetDatabaseName}...";

            var progress = new Progress<int>(p =>
            {
                Progress = p;
                StatusMessage = $"Restoring {TargetDatabaseName}... {p}%";
            });

            await _sqlServerService.RestoreDatabaseAsync(
                _connectionViewModel.ConnectionSettings,
                TargetDatabaseName,
                BackupFilePath,
                DataFilePath,
                LogFilePath,
                progress,
                _cancellationTokenSource.Token);

            Progress = 100;
            StatusMessage = $"Database {TargetDatabaseName} restored successfully!";

            // Refresh database list
            await _connectionViewModel.LoadDatabasesAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Restore cancelled.";
            Progress = 0;
        }
        catch (Exception ex)
        {
            SetError($"Restore failed: {ex.Message}");
            Progress = 0;
        }
        finally
        {
            IsRestoreInProgress = false;
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Cancels the current restore operation.
    /// </summary>
    private void CancelRestore()
    {
        _cancellationTokenSource?.Cancel();
        StatusMessage = "Cancelling restore...";
    }

    /// <summary>
    /// Opens a file browser dialog to select the backup file.
    /// </summary>
    private void BrowseBackupFile()
    {
        using var dialog = new System.Windows.Forms.OpenFileDialog
        {
            Title = "Select Backup File",
            Filter = "Backup Files (*.bak)|*.bak|Transaction Log Files (*.trn)|*.trn|All Files (*.*)|*.*",
            FilterIndex = 1,
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            BackupFilePath = dialog.FileName;
        }
    }

    /// <summary>
    /// Opens a folder browser dialog to select the data file path.
    /// </summary>
    private void BrowseDataFilePath()
    {
        using var dialog = new System.Windows.Forms.SaveFileDialog
        {
            Title = "Select Data File Location",
            Filter = "Data Files (*.mdf)|*.mdf|All Files (*.*)|*.*",
            FilterIndex = 1,
            FileName = $"{TargetDatabaseName}.mdf"
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            DataFilePath = dialog.FileName;
        }
    }

    /// <summary>
    /// Opens a folder browser dialog to select the log file path.
    /// </summary>
    private void BrowseLogFilePath()
    {
        using var dialog = new System.Windows.Forms.SaveFileDialog
        {
            Title = "Select Log File Location",
            Filter = "Log Files (*.ldf)|*.ldf|All Files (*.*)|*.*",
            FilterIndex = 1,
            FileName = $"{TargetDatabaseName}_log.ldf"
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            LogFilePath = dialog.FileName;
        }
    }
}
