using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for data transfer between SQL Server databases.
/// Implements a 3-step wizard approach.
/// </summary>
public class DataTransferViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionViewModel _connectionViewModel;
    private readonly DestinationSettingsManager _destinationSettingsManager;

    // Step tracking
    private int _currentStep = 1;

    // Step 1: Destination server
    private DestinationServerSettings _destinationSettings = new();
    private bool _isDestinationConnected;
    private string _destinationConnectionStatus = "قطع ارتباط";
    private bool _rememberDestinationSettings = true;

    // Step 2: Source data
    private DatabaseInfo? _selectedSourceDatabase;
    private DataTransferMode _transferMode = DataTransferMode.Table;
    private TableInfo? _selectedSourceTable;
    private string _customQuery = string.Empty;

    // Step 3: Destination config
    private DatabaseInfo? _selectedDestinationDatabase;
    private string _destinationTableName = string.Empty;
    private DataTransferAction _transferAction = DataTransferAction.Append;
    private DataTable? _previewData;
    private long _sourceRowCount;

    // Transfer progress
    private bool _isTransferring;
    private int _progressPercentage;
    private long _transferredRows;
    private CancellationTokenSource? _cancellationTokenSource;

    public DataTransferViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
    {
        _sqlServerService = sqlServerService;
        _connectionViewModel = connectionViewModel;
        _destinationSettingsManager = new DestinationSettingsManager();

        SourceTables = new ObservableCollection<TableInfo>();
        DestinationDatabases = new ObservableCollection<DatabaseInfo>();

        // Step 1 Commands
        TestDestinationConnectionCommand = new RelayCommand(async () => await TestDestinationConnectionAsync(), CanTestDestination);
        ConnectDestinationCommand = new RelayCommand(async () => await ConnectDestinationAsync(), CanConnectDestination);
        ClearDestinationSettingsCommand = new RelayCommand(ClearDestinationSettings);

        // Step 2 Commands
        LoadSourceTablesCommand = new RelayCommand(async () => await LoadSourceTablesAsync(), () => _connectionViewModel.IsConnected);
        PreviewDataCommand = new RelayCommand(async () => await PreviewDataAsync(), CanPreviewData);
        GetRowCountCommand = new RelayCommand(async () => await GetSourceRowCountAsync(), CanPreviewData);

        // Step 3 Commands
        TransferDataCommand = new RelayCommand(async () => await TransferDataAsync(), CanTransfer);
        CancelTransferCommand = new RelayCommand(CancelTransfer, () => IsTransferring);

        // Navigation Commands
        NextStepCommand = new RelayCommand(GoToNextStep, CanGoNext);
        PreviousStepCommand = new RelayCommand(GoToPreviousStep, () => CurrentStep > 1 && !IsTransferring);

        // Load saved destination settings
        _ = LoadSavedDestinationSettingsAsync();
    }

    #region Properties - Step Tracking

    /// <summary>
    /// Gets or sets the current wizard step (1, 2, or 3).
    /// </summary>
    public int CurrentStep
    {
        get => _currentStep;
        set
        {
            if (SetProperty(ref _currentStep, value))
            {
                OnPropertyChanged(nameof(IsStep1));
                OnPropertyChanged(nameof(IsStep2));
                OnPropertyChanged(nameof(IsStep3));
            }
        }
    }

    public bool IsStep1 => CurrentStep == 1;
    public bool IsStep2 => CurrentStep == 2;
    public bool IsStep3 => CurrentStep == 3;

    #endregion

    #region Properties - Step 1: Destination Server

    /// <summary>
    /// Gets or sets the destination server settings.
    /// </summary>
    public DestinationServerSettings DestinationSettings
    {
        get => _destinationSettings;
        set => SetProperty(ref _destinationSettings, value);
    }

    /// <summary>
    /// Gets or sets whether the destination is connected.
    /// </summary>
    public bool IsDestinationConnected
    {
        get => _isDestinationConnected;
        private set
        {
            if (SetProperty(ref _isDestinationConnected, value))
            {
                DestinationConnectionStatus = value
                    ? $"متصل شد به {DestinationSettings.ServerName}"
                    : "قطع ارتباط";
            }
        }
    }

    /// <summary>
    /// Gets or sets the destination connection status message.
    /// </summary>
    public string DestinationConnectionStatus
    {
        get => _destinationConnectionStatus;
        private set => SetProperty(ref _destinationConnectionStatus, value);
    }

    /// <summary>
    /// Gets or sets whether to remember destination settings.
    /// </summary>
    public bool RememberDestinationSettings
    {
        get => _rememberDestinationSettings;
        set => SetProperty(ref _rememberDestinationSettings, value);
    }

    #endregion

    #region Properties - Step 2: Source Data

    /// <summary>
    /// Gets the list of source databases from the connection view model.
    /// </summary>
    public ObservableCollection<DatabaseInfo> SourceDatabases => _connectionViewModel.Databases;

    /// <summary>
    /// Gets the list of source tables.
    /// </summary>
    public ObservableCollection<TableInfo> SourceTables { get; }

    /// <summary>
    /// Gets or sets the selected source database.
    /// </summary>
    public DatabaseInfo? SelectedSourceDatabase
    {
        get => _selectedSourceDatabase;
        set
        {
            if (SetProperty(ref _selectedSourceDatabase, value))
            {
                _ = LoadSourceTablesAsync();
            }
        }
    }

    /// <summary>
    /// Gets or sets the transfer mode (Table or Query).
    /// </summary>
    public DataTransferMode TransferMode
    {
        get => _transferMode;
        set
        {
            if (SetProperty(ref _transferMode, value))
            {
                OnPropertyChanged(nameof(IsTableMode));
                OnPropertyChanged(nameof(IsQueryMode));
            }
        }
    }

    public bool IsTableMode
    {
        get => TransferMode == DataTransferMode.Table;
        set { if (value) TransferMode = DataTransferMode.Table; }
    }

    public bool IsQueryMode
    {
        get => TransferMode == DataTransferMode.Query;
        set { if (value) TransferMode = DataTransferMode.Query; }
    }

    /// <summary>
    /// Gets or sets the selected source table.
    /// </summary>
    public TableInfo? SelectedSourceTable
    {
        get => _selectedSourceTable;
        set
        {
            if (SetProperty(ref _selectedSourceTable, value))
            {
                DestinationTableName = value?.Name ?? string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the custom SQL query.
    /// </summary>
    public string CustomQuery
    {
        get => _customQuery;
        set => SetProperty(ref _customQuery, value);
    }

    #endregion

    #region Properties - Step 3: Destination Configuration

    /// <summary>
    /// Gets the list of destination databases.
    /// </summary>
    public ObservableCollection<DatabaseInfo> DestinationDatabases { get; }

    /// <summary>
    /// Gets or sets the selected destination database.
    /// </summary>
    public DatabaseInfo? SelectedDestinationDatabase
    {
        get => _selectedDestinationDatabase;
        set => SetProperty(ref _selectedDestinationDatabase, value);
    }

    /// <summary>
    /// Gets or sets the destination table name.
    /// </summary>
    public string DestinationTableName
    {
        get => _destinationTableName;
        set => SetProperty(ref _destinationTableName, value);
    }

    /// <summary>
    /// Gets or sets the transfer action (Append or Replace).
    /// </summary>
    public DataTransferAction TransferAction
    {
        get => _transferAction;
        set
        {
            if (SetProperty(ref _transferAction, value))
            {
                OnPropertyChanged(nameof(IsAppendMode));
                OnPropertyChanged(nameof(IsReplaceMode));
            }
        }
    }

    public bool IsAppendMode
    {
        get => TransferAction == DataTransferAction.Append;
        set { if (value) TransferAction = DataTransferAction.Append; }
    }

    public bool IsReplaceMode
    {
        get => TransferAction == DataTransferAction.Replace;
        set { if (value) TransferAction = DataTransferAction.Replace; }
    }

    /// <summary>
    /// Gets or sets the preview data table.
    /// </summary>
    public DataTable? PreviewData
    {
        get => _previewData;
        private set => SetProperty(ref _previewData, value);
    }

    /// <summary>
    /// Gets or sets the source row count.
    /// </summary>
    public long SourceRowCount
    {
        get => _sourceRowCount;
        private set => SetProperty(ref _sourceRowCount, value);
    }

    #endregion

    #region Properties - Transfer Progress

    /// <summary>
    /// Gets or sets whether a transfer is in progress.
    /// </summary>
    public bool IsTransferring
    {
        get => _isTransferring;
        private set => SetProperty(ref _isTransferring, value);
    }

    /// <summary>
    /// Gets or sets the transfer progress percentage.
    /// </summary>
    public int ProgressPercentage
    {
        get => _progressPercentage;
        private set => SetProperty(ref _progressPercentage, value);
    }

    /// <summary>
    /// Gets or sets the number of transferred rows.
    /// </summary>
    public long TransferredRows
    {
        get => _transferredRows;
        private set => SetProperty(ref _transferredRows, value);
    }

    #endregion

    #region Commands

    // Step 1
    public ICommand TestDestinationConnectionCommand { get; }
    public ICommand ConnectDestinationCommand { get; }
    public ICommand ClearDestinationSettingsCommand { get; }

    // Step 2
    public ICommand LoadSourceTablesCommand { get; }
    public ICommand PreviewDataCommand { get; }
    public ICommand GetRowCountCommand { get; }

    // Step 3
    public ICommand TransferDataCommand { get; }
    public ICommand CancelTransferCommand { get; }

    // Navigation
    public ICommand NextStepCommand { get; }
    public ICommand PreviousStepCommand { get; }

    #endregion

    #region Command Can Execute

    private bool CanTestDestination() => !string.IsNullOrWhiteSpace(DestinationSettings.ServerName) && !IsBusy;

    private bool CanConnectDestination() => !string.IsNullOrWhiteSpace(DestinationSettings.ServerName) && !IsBusy;

    private bool CanPreviewData()
    {
        if (SelectedSourceDatabase == null || !_connectionViewModel.IsConnected) return false;
        return TransferMode == DataTransferMode.Table
            ? SelectedSourceTable != null
            : !string.IsNullOrWhiteSpace(CustomQuery);
    }

    private bool CanTransfer()
    {
        return SelectedSourceDatabase != null &&
               SelectedDestinationDatabase != null &&
               !string.IsNullOrWhiteSpace(DestinationTableName) &&
               IsDestinationConnected &&
               !IsTransferring &&
               _connectionViewModel.IsConnected &&
               (TransferMode == DataTransferMode.Table ? SelectedSourceTable != null : !string.IsNullOrWhiteSpace(CustomQuery));
    }

    private bool CanGoNext()
    {
        if (IsTransferring) return false;
        return CurrentStep switch
        {
            1 => IsDestinationConnected,
            2 => SelectedSourceDatabase != null &&
                 (TransferMode == DataTransferMode.Table ? SelectedSourceTable != null : !string.IsNullOrWhiteSpace(CustomQuery)),
            _ => false
        };
    }

    #endregion

    #region Step 1 Methods

    /// <summary>
    /// Tests the connection to the destination server.
    /// </summary>
    private async Task TestDestinationConnectionAsync()
    {
        IsBusy = true;
        ClearError();
        StatusMessage = "در حال تست اتصال به سرور مقصد...";

        try
        {
            var success = await _sqlServerService.TestDestinationConnectionAsync(DestinationSettings);
            StatusMessage = success
                ? "تست اتصال به سرور مقصد موفق بود!"
                : "تست اتصال ناموفق بود.";

            if (!success)
                SetError("تست اتصال به سرور مقصد ناموفق بود. لطفا تنظیمات را بررسی کنید.");
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
    /// Connects to the destination server and loads databases.
    /// </summary>
    private async Task ConnectDestinationAsync()
    {
        IsBusy = true;
        ClearError();
        StatusMessage = "در حال اتصال به سرور مقصد...";

        try
        {
            var success = await _sqlServerService.TestDestinationConnectionAsync(DestinationSettings);
            if (success)
            {
                IsDestinationConnected = true;

                // Load destination databases
                var databases = await _sqlServerService.GetDestinationDatabasesAsync(DestinationSettings);
                DestinationDatabases.Clear();
                foreach (var db in databases)
                {
                    DestinationDatabases.Add(db);
                }

                // Save settings if remember is enabled
                if (RememberDestinationSettings)
                {
                    await _destinationSettingsManager.SaveDestinationSettingsAsync(DestinationSettings);
                }

                StatusMessage = $"متصل شد. {DestinationDatabases.Count} پایگاه داده در سرور مقصد یافت شد.";
            }
            else
            {
                SetError("اتصال به سرور مقصد ناموفق بود. لطفا تنظیمات را بررسی کنید.");
            }
        }
        catch (Exception ex)
        {
            SetError($"اتصال ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears saved destination settings.
    /// </summary>
    private void ClearDestinationSettings()
    {
        _destinationSettingsManager.DeleteDestinationSettings();
        StatusMessage = "تنظیمات سرور مقصد پاک شد.";
    }

    /// <summary>
    /// Loads previously saved destination settings.
    /// </summary>
    private async Task LoadSavedDestinationSettingsAsync()
    {
        try
        {
            var savedSettings = await _destinationSettingsManager.LoadDestinationSettingsAsync();
            if (savedSettings != null)
            {
                DestinationSettings.ServerName = savedSettings.ServerName;
                DestinationSettings.AuthenticationType = savedSettings.AuthenticationType;
                DestinationSettings.Username = savedSettings.Username;
                DestinationSettings.Password = savedSettings.Password;
                DestinationSettings.ConnectionTimeout = savedSettings.ConnectionTimeout;
                DestinationSettings.TrustServerCertificate = savedSettings.TrustServerCertificate;
                DestinationSettings.DatabaseName = savedSettings.DatabaseName;
                StatusMessage = "تنظیمات سرور مقصد ذخیره شده بارگذاری شد.";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطا در بارگذاری تنظیمات مقصد: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the current destination password for PasswordBox population.
    /// </summary>
    public string GetDestinationPassword() => DestinationSettings.Password;

    #endregion

    #region Step 2 Methods

    /// <summary>
    /// Loads tables from the selected source database.
    /// </summary>
    private async Task LoadSourceTablesAsync()
    {
        if (SelectedSourceDatabase == null) return;

        IsBusy = true;
        ClearError();
        SourceTables.Clear();
        StatusMessage = "در حال بارگذاری جداول...";

        try
        {
            var tables = await _sqlServerService.GetTablesAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedSourceDatabase.Name);

            foreach (var table in tables)
            {
                SourceTables.Add(table);
            }

            StatusMessage = $"{SourceTables.Count} جدول بارگذاری شد.";
        }
        catch (Exception ex)
        {
            SetError($"بارگذاری جداول ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Previews data from the source.
    /// </summary>
    private async Task PreviewDataAsync()
    {
        if (SelectedSourceDatabase == null) return;

        IsBusy = true;
        ClearError();
        StatusMessage = "در حال بارگذاری پیش‌نمایش داده‌ها...";

        try
        {
            var isQuery = TransferMode == DataTransferMode.Query;
            var sourceExpression = isQuery ? CustomQuery : SelectedSourceTable?.Name ?? string.Empty;

            PreviewData = await _sqlServerService.GetDataPreviewAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedSourceDatabase.Name,
                sourceExpression,
                isQuery,
                10);

            StatusMessage = $"پیش‌نمایش {PreviewData.Rows.Count} رکورد بارگذاری شد.";
        }
        catch (Exception ex)
        {
            SetError($"پیش‌نمایش ناموفق بود: {ex.Message}");
            PreviewData = null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Gets the row count from the source.
    /// </summary>
    private async Task GetSourceRowCountAsync()
    {
        if (SelectedSourceDatabase == null) return;

        IsBusy = true;
        ClearError();
        StatusMessage = "در حال شمارش رکوردها...";

        try
        {
            var isQuery = TransferMode == DataTransferMode.Query;
            var sourceExpression = isQuery ? CustomQuery : SelectedSourceTable?.Name ?? string.Empty;

            SourceRowCount = await _sqlServerService.GetRowCountAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedSourceDatabase.Name,
                sourceExpression,
                isQuery);

            StatusMessage = $"تعداد رکوردها: {SourceRowCount}";
        }
        catch (Exception ex)
        {
            SetError($"شمارش رکوردها ناموفق بود: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Step 3 Methods

    /// <summary>
    /// Transfers data from source to destination.
    /// </summary>
    private async Task TransferDataAsync()
    {
        if (SelectedSourceDatabase == null || SelectedDestinationDatabase == null) return;

        IsTransferring = true;
        IsBusy = true;
        ProgressPercentage = 0;
        TransferredRows = 0;
        ClearError();

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var request = new DataTransferRequest
            {
                SourceDatabaseName = SelectedSourceDatabase.Name,
                TransferMode = TransferMode,
                SourceTableName = SelectedSourceTable?.Name ?? string.Empty,
                CustomQuery = CustomQuery,
                DestinationTableName = DestinationTableName,
                DestinationDatabaseName = SelectedDestinationDatabase.Name,
                TransferAction = TransferAction
            };

            var progress = new Progress<TransferProgress>(p =>
            {
                ProgressPercentage = p.ProgressPercentage;
                TransferredRows = p.TransferredRows;
                StatusMessage = p.StatusMessage;
            });

            await _sqlServerService.TransferDataAsync(
                _connectionViewModel.ConnectionSettings,
                DestinationSettings,
                request,
                progress,
                _cancellationTokenSource.Token);

            ProgressPercentage = 100;
            StatusMessage = $"انتقال با موفقیت انجام شد. {TransferredRows} رکورد منتقل شد.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "انتقال لغو شد. تغییرات بازگردانی شد.";
            ProgressPercentage = 0;
        }
        catch (Exception ex)
        {
            SetError($"انتقال ناموفق بود: {ex.Message}");
            ProgressPercentage = 0;
        }
        finally
        {
            IsTransferring = false;
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Cancels the current transfer operation.
    /// </summary>
    private void CancelTransfer()
    {
        _cancellationTokenSource?.Cancel();
        StatusMessage = "در حال لغو انتقال...";
    }

    #endregion

    #region Navigation

    private void GoToNextStep()
    {
        if (CurrentStep < 3)
            CurrentStep++;
    }

    private void GoToPreviousStep()
    {
        if (CurrentStep > 1)
            CurrentStep--;
    }

    #endregion
}
