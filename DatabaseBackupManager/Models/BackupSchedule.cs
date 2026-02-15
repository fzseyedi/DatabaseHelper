using DatabaseBackupManager.Helpers;

namespace DatabaseBackupManager.Models;

/// <summary>
/// Represents a scheduled backup configuration.
/// </summary>
public class BackupSchedule : ObservableObject
{
    private string _name = string.Empty;
    private string _databaseName = string.Empty;
    private BackupType _backupType = BackupType.Full;
    private ScheduleFrequency _frequency = ScheduleFrequency.Daily;
    private TimeSpan _scheduledTime = new(2, 0, 0); // Default 2:00 AM
    private DayOfWeek _dayOfWeek = DayOfWeek.Sunday;
    private int _dayOfMonth = 1;
    private string _backupPath = string.Empty;
    private bool _isEnabled = true;
    private DateTime? _lastRunDate;
    private DateTime? _nextRunDate;
    private bool _compressBackup = true;
    private int _retentionDays = 30;

    /// <summary>
    /// Gets or sets the schedule name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the database name to backup.
    /// </summary>
    public string DatabaseName
    {
        get => _databaseName;
        set => SetProperty(ref _databaseName, value);
    }

    /// <summary>
    /// Gets or sets the backup type.
    /// </summary>
    public BackupType BackupType
    {
        get => _backupType;
        set => SetProperty(ref _backupType, value);
    }

    /// <summary>
    /// Gets or sets the schedule frequency.
    /// </summary>
    public ScheduleFrequency Frequency
    {
        get => _frequency;
        set => SetProperty(ref _frequency, value);
    }

    /// <summary>
    /// Gets or sets the time of day to run the backup.
    /// </summary>
    public TimeSpan ScheduledTime
    {
        get => _scheduledTime;
        set => SetProperty(ref _scheduledTime, value);
    }

    /// <summary>
    /// Gets or sets the day of week for weekly backups.
    /// </summary>
    public DayOfWeek DayOfWeek
    {
        get => _dayOfWeek;
        set => SetProperty(ref _dayOfWeek, value);
    }

    /// <summary>
    /// Gets or sets the day of month for monthly backups.
    /// </summary>
    public int DayOfMonth
    {
        get => _dayOfMonth;
        set => SetProperty(ref _dayOfMonth, value);
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
    /// Gets or sets whether the schedule is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    /// <summary>
    /// Gets or sets the last run date.
    /// </summary>
    public DateTime? LastRunDate
    {
        get => _lastRunDate;
        set => SetProperty(ref _lastRunDate, value);
    }

    /// <summary>
    /// Gets or sets the next scheduled run date.
    /// </summary>
    public DateTime? NextRunDate
    {
        get => _nextRunDate;
        set => SetProperty(ref _nextRunDate, value);
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
    /// Gets or sets the number of days to retain backups.
    /// </summary>
    public int RetentionDays
    {
        get => _retentionDays;
        set => SetProperty(ref _retentionDays, value);
    }

    /// <summary>
    /// Calculates the next run date based on the schedule configuration.
    /// </summary>
    public void CalculateNextRunDate()
    {
        var now = DateTime.Now;
        var today = now.Date.Add(ScheduledTime);

        NextRunDate = Frequency switch
        {
            ScheduleFrequency.Daily => today > now ? today : today.AddDays(1),
            ScheduleFrequency.Weekly => GetNextWeeklyDate(now),
            ScheduleFrequency.Monthly => GetNextMonthlyDate(now),
            _ => today.AddDays(1)
        };
    }

    private DateTime GetNextWeeklyDate(DateTime now)
    {
        var daysUntilTarget = ((int)DayOfWeek - (int)now.DayOfWeek + 7) % 7;
        var nextDate = now.Date.AddDays(daysUntilTarget).Add(ScheduledTime);
        
        if (nextDate <= now)
            nextDate = nextDate.AddDays(7);
        
        return nextDate;
    }

    private DateTime GetNextMonthlyDate(DateTime now)
    {
        var day = Math.Min(DayOfMonth, DateTime.DaysInMonth(now.Year, now.Month));
        var nextDate = new DateTime(now.Year, now.Month, day).Add(ScheduledTime);
        
        if (nextDate <= now)
        {
            var nextMonth = now.AddMonths(1);
            day = Math.Min(DayOfMonth, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            nextDate = new DateTime(nextMonth.Year, nextMonth.Month, day).Add(ScheduledTime);
        }
        
        return nextDate;
    }
}

/// <summary>
/// Types of database backups.
/// </summary>
public enum BackupType
{
    /// <summary>
    /// Full database backup.
    /// </summary>
    Full,

    /// <summary>
    /// Differential backup (changes since last full backup).
    /// </summary>
    Differential,

    /// <summary>
    /// Transaction log backup.
    /// </summary>
    TransactionLog
}

/// <summary>
/// Backup schedule frequency options.
/// </summary>
public enum ScheduleFrequency
{
    /// <summary>
    /// Run backup every day.
    /// </summary>
    Daily,

    /// <summary>
    /// Run backup once a week.
    /// </summary>
    Weekly,

    /// <summary>
    /// Run backup once a month.
    /// </summary>
    Monthly
}
