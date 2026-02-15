using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using DatabaseBackupManager.Commands;
using DatabaseBackupManager.Models;
using DatabaseBackupManager.Services;

namespace DatabaseBackupManager.ViewModels;

/// <summary>
/// ViewModel for managing backup schedules.
/// </summary>
public class ScheduleViewModel : ViewModelBase
{
    private readonly ISqlServerService _sqlServerService;
    private readonly ConnectionViewModel _connectionViewModel;
    private BackupSchedule? _selectedSchedule;
    private BackupSchedule _editingSchedule;
    private bool _isEditing;
    private DatabaseInfo? _selectedDatabase;
    private readonly string _schedulesFilePath;

    public ScheduleViewModel(ISqlServerService sqlServerService, ConnectionViewModel connectionViewModel)
    {
        _sqlServerService = sqlServerService;
        _connectionViewModel = connectionViewModel;
        _editingSchedule = CreateNewSchedule();

        Schedules = [];

        // Set schedules file path in user's app data
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "DatabaseBackupManager");
        Directory.CreateDirectory(appFolder);
        _schedulesFilePath = Path.Combine(appFolder, "schedules.json");

        // Initialize commands
        AddScheduleCommand = new RelayCommand(AddSchedule, CanAddSchedule);
        EditScheduleCommand = new RelayCommand(EditSchedule, () => SelectedSchedule != null);
        DeleteScheduleCommand = new RelayCommand(DeleteSchedule, () => SelectedSchedule != null);
        SaveScheduleCommand = new RelayCommand(async () => await SaveScheduleAsync(), CanSaveSchedule);
        CancelEditCommand = new RelayCommand(CancelEdit, () => IsEditing);
        ToggleScheduleCommand = new RelayCommand(ToggleSchedule, () => SelectedSchedule != null);
        BrowseBackupPathCommand = new RelayCommand(BrowseBackupPath);
        RunNowCommand = new RelayCommand(async () => await RunScheduleNowAsync(), () => SelectedSchedule != null && _connectionViewModel.IsConnected);
        LoadSchedulesCommand = new RelayCommand(async () => await LoadSchedulesAsync());

        // Load schedules on initialization
        _ = LoadSchedulesAsync();
    }

    /// <summary>
    /// Gets the collection of backup schedules.
    /// </summary>
    public ObservableCollection<BackupSchedule> Schedules { get; }

    /// <summary>
    /// Gets the list of databases from the connection view model.
    /// </summary>
    public ObservableCollection<DatabaseInfo> Databases => _connectionViewModel.Databases;

    /// <summary>
    /// Gets or sets the selected schedule.
    /// </summary>
    public BackupSchedule? SelectedSchedule
    {
        get => _selectedSchedule;
        set => SetProperty(ref _selectedSchedule, value);
    }

    /// <summary>
    /// Gets or sets the schedule being edited.
    /// </summary>
    public BackupSchedule EditingSchedule
    {
        get => _editingSchedule;
        set => SetProperty(ref _editingSchedule, value);
    }

    /// <summary>
    /// Gets or sets the selected database for the editing schedule.
    /// </summary>
    public DatabaseInfo? SelectedDatabase
    {
        get => _selectedDatabase;
        set
        {
            if (SetProperty(ref _selectedDatabase, value) && value != null)
            {
                EditingSchedule.DatabaseName = value.Name;
            }
        }
    }

    /// <summary>
    /// Gets or sets whether a schedule is currently being edited.
    /// </summary>
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    /// <summary>
    /// Gets the available backup types.
    /// </summary>
    public IEnumerable<BackupType> BackupTypes => Enum.GetValues<BackupType>();

    /// <summary>
    /// Gets the available schedule frequencies.
    /// </summary>
    public IEnumerable<ScheduleFrequency> Frequencies => Enum.GetValues<ScheduleFrequency>();

    /// <summary>
    /// Gets the days of the week.
    /// </summary>
    public IEnumerable<DayOfWeek> DaysOfWeek => Enum.GetValues<DayOfWeek>();

    /// <summary>
    /// Gets the days of the month (1-31).
    /// </summary>
    public IEnumerable<int> DaysOfMonth => Enumerable.Range(1, 31);

    // Commands
    public ICommand AddScheduleCommand { get; }
    public ICommand EditScheduleCommand { get; }
    public ICommand DeleteScheduleCommand { get; }
    public ICommand SaveScheduleCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand ToggleScheduleCommand { get; }
    public ICommand BrowseBackupPathCommand { get; }
    public ICommand RunNowCommand { get; }
    public ICommand LoadSchedulesCommand { get; }

    private bool CanAddSchedule() => !IsEditing;

    private bool CanSaveSchedule() =>
        IsEditing &&
        !string.IsNullOrWhiteSpace(EditingSchedule.Name) &&
        !string.IsNullOrWhiteSpace(EditingSchedule.DatabaseName) &&
        !string.IsNullOrWhiteSpace(EditingSchedule.BackupPath);

    /// <summary>
    /// Creates a new schedule template.
    /// </summary>
    private static BackupSchedule CreateNewSchedule()
    {
        return new BackupSchedule
        {
            Name = string.Empty,
            BackupType = BackupType.Full,
            Frequency = ScheduleFrequency.Daily,
            ScheduledTime = new TimeSpan(2, 0, 0), // 2:00 AM
            IsEnabled = true,
            CompressBackup = true,
            RetentionDays = 30,
            BackupPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
    }

    /// <summary>
    /// Prepares a new schedule for editing.
    /// </summary>
    private void AddSchedule()
    {
        EditingSchedule = CreateNewSchedule();
        SelectedDatabase = null;
        IsEditing = true;
        StatusMessage = "Creating new schedule...";
    }

    /// <summary>
    /// Loads the selected schedule for editing.
    /// </summary>
    private void EditSchedule()
    {
        if (SelectedSchedule == null) return;

        // Clone the selected schedule for editing
        EditingSchedule = new BackupSchedule
        {
            Name = SelectedSchedule.Name,
            DatabaseName = SelectedSchedule.DatabaseName,
            BackupType = SelectedSchedule.BackupType,
            Frequency = SelectedSchedule.Frequency,
            ScheduledTime = SelectedSchedule.ScheduledTime,
            DayOfWeek = SelectedSchedule.DayOfWeek,
            DayOfMonth = SelectedSchedule.DayOfMonth,
            BackupPath = SelectedSchedule.BackupPath,
            IsEnabled = SelectedSchedule.IsEnabled,
            CompressBackup = SelectedSchedule.CompressBackup,
            RetentionDays = SelectedSchedule.RetentionDays
        };

        // Try to find matching database
        SelectedDatabase = Databases.FirstOrDefault(d => d.Name == SelectedSchedule.DatabaseName);

        IsEditing = true;
        StatusMessage = $"Editing schedule: {SelectedSchedule.Name}";
    }

    /// <summary>
    /// Deletes the selected schedule.
    /// </summary>
    private void DeleteSchedule()
    {
        if (SelectedSchedule == null) return;

        var scheduleName = SelectedSchedule.Name;
        Schedules.Remove(SelectedSchedule);
        SelectedSchedule = null;

        _ = SaveSchedulesToFileAsync();

        StatusMessage = $"Schedule '{scheduleName}' deleted.";
    }

    /// <summary>
    /// Saves the editing schedule.
    /// </summary>
    private async Task SaveScheduleAsync()
    {
        if (!CanSaveSchedule()) return;

        IsBusy = true;
        ClearError();

        try
        {
            // Check if this is an update to existing schedule
            var existingSchedule = Schedules.FirstOrDefault(s => s.Name == EditingSchedule.Name);

            if (existingSchedule != null && existingSchedule != SelectedSchedule)
            {
                SetError("A schedule with this name already exists.");
                return;
            }

            // If editing existing, update it
            if (SelectedSchedule != null)
            {
                var index = Schedules.IndexOf(SelectedSchedule);
                if (index >= 0)
                {
                    Schedules.RemoveAt(index);
                    EditingSchedule.CalculateNextRunDate();
                    Schedules.Insert(index, EditingSchedule);
                }
            }
            else
            {
                // Add new schedule
                EditingSchedule.CalculateNextRunDate();
                Schedules.Add(EditingSchedule);
            }

            await SaveSchedulesToFileAsync();

            IsEditing = false;
            EditingSchedule = CreateNewSchedule();
            StatusMessage = "Schedule saved successfully.";
        }
        catch (Exception ex)
        {
            SetError($"Failed to save schedule: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Cancels the current edit operation.
    /// </summary>
    private void CancelEdit()
    {
        IsEditing = false;
        EditingSchedule = CreateNewSchedule();
        SelectedDatabase = null;
        StatusMessage = "Edit cancelled.";
    }

    /// <summary>
    /// Toggles the enabled state of the selected schedule.
    /// </summary>
    private void ToggleSchedule()
    {
        if (SelectedSchedule == null) return;

        SelectedSchedule.IsEnabled = !SelectedSchedule.IsEnabled;

        if (SelectedSchedule.IsEnabled)
        {
            SelectedSchedule.CalculateNextRunDate();
        }

        _ = SaveSchedulesToFileAsync();

        StatusMessage = SelectedSchedule.IsEnabled
            ? $"Schedule '{SelectedSchedule.Name}' enabled."
            : $"Schedule '{SelectedSchedule.Name}' disabled.";
    }

    /// <summary>
    /// Opens a folder browser dialog to select the backup path.
    /// </summary>
    private void BrowseBackupPath()
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select Backup Destination Folder",
            SelectedPath = EditingSchedule.BackupPath,
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            EditingSchedule.BackupPath = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// Runs the selected schedule immediately.
    /// </summary>
    private async Task RunScheduleNowAsync()
    {
        if (SelectedSchedule == null || !_connectionViewModel.IsConnected) return;

        IsBusy = true;
        ClearError();

        try
        {
            StatusMessage = $"Running backup for '{SelectedSchedule.DatabaseName}'...";

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var extension = SelectedSchedule.BackupType == BackupType.TransactionLog ? ".trn" : ".bak";
            var fileName = $"{SelectedSchedule.DatabaseName}_{SelectedSchedule.BackupType}_{timestamp}{extension}";
            var fullPath = Path.Combine(SelectedSchedule.BackupPath, fileName);

            await _sqlServerService.BackupDatabaseAsync(
                _connectionViewModel.ConnectionSettings,
                SelectedSchedule.DatabaseName,
                fullPath,
                SelectedSchedule.BackupType,
                SelectedSchedule.CompressBackup);

            SelectedSchedule.LastRunDate = DateTime.Now;
            SelectedSchedule.CalculateNextRunDate();

            await SaveSchedulesToFileAsync();

            StatusMessage = $"Backup completed: {fileName}";
        }
        catch (Exception ex)
        {
            SetError($"Backup failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads schedules from the file.
    /// </summary>
    private async Task LoadSchedulesAsync()
    {
        if (!File.Exists(_schedulesFilePath)) return;

        IsBusy = true;

        try
        {
            var json = await File.ReadAllTextAsync(_schedulesFilePath);
            var schedules = JsonSerializer.Deserialize<List<BackupSchedule>>(json);

            Schedules.Clear();

            if (schedules != null)
            {
                foreach (var schedule in schedules)
                {
                    if (schedule.IsEnabled)
                    {
                        schedule.CalculateNextRunDate();
                    }
                    Schedules.Add(schedule);
                }
            }

            StatusMessage = $"Loaded {Schedules.Count} schedule(s).";
        }
        catch (Exception ex)
        {
            SetError($"Failed to load schedules: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves schedules to the file.
    /// </summary>
    private async Task SaveSchedulesToFileAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Schedules.ToList(), options);
            await File.WriteAllTextAsync(_schedulesFilePath, json);
        }
        catch (Exception ex)
        {
            SetError($"Failed to save schedules: {ex.Message}");
        }
    }
}
