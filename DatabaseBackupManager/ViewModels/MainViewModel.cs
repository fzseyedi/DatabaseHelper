using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// Main ViewModel that handles navigation between views.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel = null!;
    private readonly ConnectionViewModel _connectionViewModel;
    private readonly BackupViewModel _backupViewModel;
    private readonly RestoreViewModel _restoreViewModel;
    private readonly ScheduleViewModel _scheduleViewModel;
    private readonly FieldManagerViewModel _fieldManagerViewModel;
    private readonly DataTransferViewModel _dataTransferViewModel;

    public MainViewModel(ISqlServerService sqlServerService)
    {
        // Initialize ViewModels
        _connectionViewModel = new ConnectionViewModel(sqlServerService);
        _backupViewModel = new BackupViewModel(sqlServerService, _connectionViewModel);
        _restoreViewModel = new RestoreViewModel(sqlServerService, _connectionViewModel);
        _scheduleViewModel = new ScheduleViewModel(sqlServerService, _connectionViewModel);
        _fieldManagerViewModel = new FieldManagerViewModel(sqlServerService, _connectionViewModel);
        _dataTransferViewModel = new DataTransferViewModel(sqlServerService, _connectionViewModel);

        // Set default view
        CurrentViewModel = _connectionViewModel;

        // Initialize commands
        NavigateToConnectionCommand = new RelayCommand(() => CurrentViewModel = _connectionViewModel);
        NavigateToBackupCommand = new RelayCommand(() => CurrentViewModel = _backupViewModel, CanNavigateToBackup);
        NavigateToRestoreCommand = new RelayCommand(() => CurrentViewModel = _restoreViewModel, CanNavigateToRestore);
        NavigateToScheduleCommand = new RelayCommand(() => CurrentViewModel = _scheduleViewModel, CanNavigateToSchedule);
        NavigateToFieldManagerCommand = new RelayCommand(() => CurrentViewModel = _fieldManagerViewModel, CanNavigateToFieldManager);
        NavigateToDataTransferCommand = new RelayCommand(() => CurrentViewModel = _dataTransferViewModel, CanNavigateToDataTransfer);

        // Subscribe to connection changes
        _connectionViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ConnectionViewModel.IsConnected))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        };
    }

    /// <summary>
    /// Gets or sets the current view model being displayed.
    /// </summary>
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    /// <summary>
    /// Gets the connection view model.
    /// </summary>
    public ConnectionViewModel ConnectionViewModel => _connectionViewModel;

    /// <summary>
    /// Gets the backup view model.
    /// </summary>
    public BackupViewModel BackupViewModel => _backupViewModel;

    /// <summary>
    /// Gets the restore view model.
    /// </summary>
    public RestoreViewModel RestoreViewModel => _restoreViewModel;

    /// <summary>
    /// Gets the schedule view model.
    /// </summary>
    public ScheduleViewModel ScheduleViewModel => _scheduleViewModel;

    /// <summary>
    /// Gets the field manager view model.
    /// </summary>
    public FieldManagerViewModel FieldManagerViewModel => _fieldManagerViewModel;

    /// <summary>
    /// Gets the data transfer view model.
    /// </summary>
    public DataTransferViewModel DataTransferViewModel => _dataTransferViewModel;

    // Navigation Commands
    public ICommand NavigateToConnectionCommand { get; }
    public ICommand NavigateToBackupCommand { get; }
    public ICommand NavigateToRestoreCommand { get; }
    public ICommand NavigateToScheduleCommand { get; }
    public ICommand NavigateToFieldManagerCommand { get; }
    public ICommand NavigateToDataTransferCommand { get; }

    private bool CanNavigateToBackup() => _connectionViewModel.IsConnected;
    private bool CanNavigateToRestore() => _connectionViewModel.IsConnected;
    private bool CanNavigateToSchedule() => _connectionViewModel.IsConnected;
    private bool CanNavigateToFieldManager() => _connectionViewModel.IsConnected;
    private bool CanNavigateToDataTransfer() => _connectionViewModel.IsConnected;
}
