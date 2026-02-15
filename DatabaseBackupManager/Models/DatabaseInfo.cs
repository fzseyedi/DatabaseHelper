using DatabaseBackupManager.Helpers;

namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents information about a SQL Server database.
/// </summary>
public class DatabaseInfo : ObservableObject
{
    private string _name = string.Empty;
    private long _sizeInMB;
    private string _status = string.Empty;
    private DateTime? _lastBackupDate;
    private string _recoveryModel = string.Empty;
    private bool _isSelected;

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the database size in megabytes.
    /// </summary>
    public long SizeInMB
    {
        get => _sizeInMB;
        set => SetProperty(ref _sizeInMB, value);
    }

    /// <summary>
    /// Gets the formatted size string.
    /// </summary>
    public string SizeFormatted => SizeInMB >= 1024 
        ? $"{SizeInMB / 1024.0:F2} GB" 
        : $"{SizeInMB} MB";

    /// <summary>
    /// Gets or sets the database status (ONLINE, OFFLINE, etc.).
    /// </summary>
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    /// <summary>
    /// Gets or sets the last backup date.
    /// </summary>
    public DateTime? LastBackupDate
    {
        get => _lastBackupDate;
        set => SetProperty(ref _lastBackupDate, value);
    }

    /// <summary>
    /// Gets the formatted last backup date string.
    /// </summary>
    public string LastBackupFormatted => LastBackupDate.HasValue 
        ? LastBackupDate.Value.ToString("yyyy-MM-dd HH:mm:ss") 
        : "Never";

    /// <summary>
    /// Gets or sets the recovery model (FULL, SIMPLE, BULK_LOGGED).
    /// </summary>
    public string RecoveryModel
    {
        get => _recoveryModel;
        set => SetProperty(ref _recoveryModel, value);
    }

    /// <summary>
    /// Gets or sets whether the database is selected for backup/restore.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
